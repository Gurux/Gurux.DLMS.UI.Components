// --------------------------------------------------------------------------
// Gurux Ltd
//
// Copyright (c) Gurux Ltd.
//
// This file is part of Gurux Device Framework.
//
// Commit: $Format:%H$
// Date:   $Format:%cI$
//
// This software is available under either of the following licenses:
//
// 1. GNU General Public License, version 2 only (GPL-2.0-only).
//    See the LICENSE file included with this distribution.
//
// 2. A commercial license granted by Gurux Ltd under a separate written
//    agreement. Use under this option requires a valid commercial license
//    and is governed by the terms of that agreement.
//
// If you have not obtained a commercial license from Gurux Ltd,
// use of this software is governed by GPL-2.0-only.
//
// Commercial licensing: https://www.gurux.fi/
//
// SPDX-License-Identifier: GPL-2.0-only OR LicenseRef-Gurux-Commercial
// --------------------------------------------------------------------------

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Tracks pending writes separately from values last read from the meter.</summary>
/// <remarks>State is shared per model instance. The host owns meter communication
/// and must serialize access; this class is not thread-safe. Editing stages dirty
/// attributes without sending messages or updating confirmed model values.</remarks>
public sealed class GXDLMSObjectEditState
{
    private static readonly ConditionalWeakTable<GXDLMSObject, GXDLMSObjectEditState> Sessions = new();
    private readonly Dictionary<int, string> _errors = new();
    private readonly Dictionary<int, string> _drafts = new();
    private readonly Dictionary<int, object?> _confirmedAttributes = new();
    private readonly Dictionary<string, string> _editorTexts = new();
    /// <summary>Gets the model whose dirty attributes hold pending writes.</summary>
    public GXDLMSObject Target { get; }
    /// <summary>Raised synchronously after an edit, acknowledgement, or refresh.</summary>
    public event Action? Changed;
    /// <summary>Gets whether dirty attributes or validation errors remain.</summary>
    public bool HasChanges => Target.GetDirtyAttributeIndexes().Length != 0 || _errors.Count != 0;
    /// <summary>Gets whether all recorded attribute inputs are valid.</summary>
    public bool IsValid => _errors.Count == 0;
    /// <summary>Gets the formatted method result, or null if none is recorded.</summary>
    public string? LastActionResult { get; private set; }
    /// <summary>Records a method result without raising Changed.</summary>
    /// <param name="value">Decoded result, or null to clear the result.</param>
    public void SetActionResult(object? value) => LastActionResult = value is null ? null : Format(value);
    /// <summary>Gets a validation error for an attribute.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <returns>The error message, or null when no error is recorded.</returns>
    public string? Error(int index) => _errors.GetValueOrDefault(index);
    /// <summary>Gets the shared state for a model instance.</summary>
    /// <param name="target">Model instance; equal logical names do not share state.</param>
    /// <returns>The state associated with this instance.</returns>
    /// <exception cref="ArgumentNullException">The target is null.</exception>
    public static GXDLMSObjectEditState For(GXDLMSObject target) => Sessions.GetValue(target, item => new(item));
    private GXDLMSObjectEditState(GXDLMSObject target) => Target = target;
    /// <summary>Keep a received value before CLR object conversion can normalize it.
    /// The host calls this for each admitted read, alongside updating the target.
    /// Drafts remain independent, and callers retain ownership of their buffers.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <param name="value">Decoded wire value; null represents a confirmed empty value.</param>
    /// <remarks>Recursively copies GXArray, GXStructure and byte buffers; other values
    /// retain their references. Does not update the model, clear dirty attributes or
    /// raise Changed. Update the model separately, then call RefreshConfirmedValues.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">The index is outside the object's attribute range.</exception>
    public void SetConfirmedAttribute(int index, object? value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, ((IGXDLMSBase)Target).GetAttributeCount());
        _confirmedAttributes[index] = GXWireValue.Copy(value);
    }
    internal bool TryGetConfirmedAttribute(int index, out object? value)
    {
        if (_confirmedAttributes.TryGetValue(index, out var stored)) { value = GXWireValue.Copy(stored); return true; }
        value = null;
        return false;
    }
    private object? OriginalValue => Target.GetValues().ElementAtOrDefault(1);
    /// <summary>Gets a pending value, falling back to the CLR model value without copying.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <returns>The pending or model value.</returns>
    public object? AttributeValue(int index) => Target.GetDirty(index, out var pending) ? pending : Target.GetValues().ElementAtOrDefault(index - 1);
    /// <summary>Gets draft text, or the confirmed value formatted for display.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <returns>Display text, including a diagnostic if conversion fails.</returns>
    public string AttributeText(int index)
    {
        if (_drafts.TryGetValue(index, out var draft)) return draft;
        try { return Format(ToUI(GXDLMSAttributeCodec.Read(Target, index), index)); }
        catch (Exception ex) when (ex is not OutOfMemoryException) { return $"Unable to display attribute: {ex.Message}"; }
    }
    /// <summary>Checks whether the wire or UI type requires collection XML.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <returns>True for array or structure attributes.</returns>
    public bool IsXmlAttribute(int index) => Target.GetDataType(index) is DataType.Array or DataType.Structure
        || Target.GetUIDataType(index) is DataType.Array or DataType.Structure;
    /// <summary>Formats a pending or confirmed attribute as table rows.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <returns>Formatted cells, or a diagnostic row if reading fails.</returns>
    public IEnumerable<IReadOnlyList<string>> AttributeRows(int index)
    {
        object? value;
        try { value = Target.GetDirty(index, out var pending) ? pending : GXDLMSAttributeCodec.Read(Target, index); }
        catch (Exception ex) when (ex is not OutOfMemoryException) { return [[ex.Message]]; }
        if (value is not System.Collections.IEnumerable rows || value is string or byte[]) return [[Format(value)]];
        return rows.Cast<object?>().Select(row => row is System.Collections.IEnumerable cells && row is not (string or byte[])
            ? (IReadOnlyList<string>)cells.Cast<object?>().Select(Format).ToArray() : [Format(row)]).ToArray();
    }
    /// <summary>Gets pending attribute 2, falling back to the model, without copying.</summary>
    public object? CurrentValue => Target.GetDirty(2, out var value) ? value : OriginalValue;
    /// <summary>Gets auxiliary editor text.</summary>
    /// <param name="key">Editor-specific key.</param>
    /// <returns>Stored text, or null if absent.</returns>
    public string? GetEditorText(string key) => _editorTexts.GetValueOrDefault(key);
    /// <summary>Stores auxiliary editor text without raising Changed.</summary>
    /// <param name="key">Editor-specific key.</param>
    /// <param name="text">Text retained until the value editor is reset.</param>
    public void SetEditorText(string key, string text) => _editorTexts[key] = text;

    /// <summary>Records an attribute 2 validation error and raises Changed.</summary>
    /// <param name="message">User-facing validation message.</param>
    public void SetValueError(string message)
    {
        _errors[2] = message;
        Changed?.Invoke();
    }

    /// <summary>Stages an already converted attribute 2 value without type validation.</summary>
    /// <param name="value">Value in the meter's representation, validated by the caller.</param>
    /// <remarks>Clears the attribute error and raises Changed. No meter communication occurs.</remarks>
    public void EditMeterValue(object value)
    {
        _drafts[2] = Format(value);
        if (Format(value) == Format(OriginalValue)) Target.ClearDirty(2);
        else Target.UpdateDirty(2, value);
        _errors.Remove(2);
        Changed?.Invoke();
    }

    /// <summary>Accepts an acknowledged Data or Register attribute 2 write.</summary>
    /// <param name="value">Value confirmed by the meter.</param>
    /// <remarks>Call only after successful meter acknowledgement. Updates the model and
    /// confirmed snapshot, clears attribute 2's dirty flag, draft, error and auxiliary
    /// editor texts, then raises Changed. Intended for GXDLMSData and GXDLMSRegister.</remarks>
    public void AcceptValue(object value)
    {
        if (Target is GXDLMSData data) data.Value = value;
        else if (Target is GXDLMSRegister register) register.Value = value;
        SetConfirmedAttribute(2, value);
        Target.ClearDirty(2);
        _drafts.Remove(2);
        _editorTexts.Clear();
        _errors.Remove(2);
        Changed?.Invoke();
    }
    /// <summary>Discard editor drafts only for attributes already confirmed by the connection service.</summary>
    /// <remarks>The host must first record received values and update the model.
    /// Only drafts with neither a dirty value nor an error are discarded. Pending
    /// and invalid inputs survive incoming reads. Always raises Changed.</remarks>
    public void RefreshConfirmedValues()
    {
        foreach (var index in _drafts.Keys.ToArray())
            if (!Target.GetDirty(index, out _) && !_errors.ContainsKey(index))
            {
                _drafts.Remove(index);
                if (index == 2) _editorTexts.Clear();
                if (index == 3) _unit = null;
            }
        Changed?.Invoke();
    }
    /// <summary>Gets draft or confirmed display text for attribute 2.</summary>
    public string ValueText => AttributeText(2);
    /// <summary>Gets draft or invariant scaler text. Requires a GXDLMSRegister target.</summary>
    public string ScalerText => _drafts.GetValueOrDefault(3) ?? ((GXDLMSRegister)Target).Scaler.ToString(CultureInfo.InvariantCulture);
    private Unit? _unit;
    /// <summary>Gets the draft unit, register model unit, or None for other objects.</summary>
    public Unit Unit => _unit ?? (Target is GXDLMSRegister register ? register.Unit : Enums.Unit.None);
    /// <summary>Gets whether attribute 2 has a collection type or collection model value.</summary>
    public bool IsXml => Target.GetDataType(2) is DataType.Array or DataType.Structure
        || Target.GetUIDataType(2) is DataType.Array or DataType.Structure || OriginalValue is GXArray or GXStructure;

    private object? ToUI(object? value, int index = 2)
    {
        var ui = Target.GetUIDataType(index);
        if (value is byte[] bytes && ui is not DataType.None and not DataType.OctetString)
            return GXDLMSClient.ChangeType(bytes, ui);
        return value;
    }

    /// <summary>Validates and stages attribute 2 text using EditAttribute.</summary>
    /// <param name="text">Invariant scalar text or DLMS translator collection XML.</param>
    public void EditValue(string text) => EditAttribute(2, text);

    /// <summary>Validates text and stages a dirty attribute without changing confirmed values.</summary>
    /// <param name="index">One-based COSEM attribute index.</param>
    /// <param name="text">Invariant scalar text or DLMS translator array/structure XML.</param>
    /// <remarks>Invalid input is retained with an error and cannot remain a pending
    /// write. Returning to the confirmed value clears the dirty flag. XML DTDs are
    /// prohibited. Raises Changed after validation. The caller checks access rights.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">The index is outside the model's attribute range.</exception>
    public void EditAttribute(int index, string text)
    {
        var originalValues = Target.GetValues();
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, originalValues.Length);
        _drafts[index] = text;
        const string defaultError = "Invalid value for the selected data type.";
        var validationError = defaultError;
        try
        {
            var original = GXDLMSAttributeCodec.Read(Target, index);
            var wire = Target.GetDataType(index);
            var ui = Target.GetUIDataType(index);
            var type = ui != DataType.None ? ui : wire == DataType.OctetString ? original switch
            {
                GXDate => DataType.Date, GXTime => DataType.Time, GXDateTime or DateTime => DataType.DateTime,
                string => DataType.String, _ => wire
            } : wire;
            object? value;
            if (Target is GXDLMSIp4Setup && index is 3 or 6 or 7 or 9 or 10
                || Target is GXDLMSIp6Setup && index is 7 or 8)
            {
                var address = System.Net.IPAddress.Parse(text);
                var family = Target is GXDLMSIp4Setup ? System.Net.Sockets.AddressFamily.InterNetwork : System.Net.Sockets.AddressFamily.InterNetworkV6;
                if (address.AddressFamily != family) throw new FormatException("Wrong IP address family.");
                value = address.ToString();
            }
            else if (Target is GXDLMSCoAPSetup or GXDLMSMBusPortSetup && index == 2)
            {
                var parts = text.Split('.');
                if (parts.Length != 6)
                {
                    throw new FormatException("A logical name requires six byte values.");
                }
                value = parts.Select(part => byte.Parse(part, NumberStyles.None, CultureInfo.InvariantCulture)).ToArray();
            }
            else if (Target is GXDLMSGprsSetup && index == 2)
            {
                value = text;
            }
            else if (wire is DataType.Array or DataType.Structure || type is DataType.Array or DataType.Structure || original is GXArray or GXStructure)
            {
                using var reader = XmlReader.Create(new StringReader(text), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
                var document = XDocument.Load(reader);
                value = GXDLMSTranslator.XmlToValue(document.ToString());
                // CoAP capture time requires GXDateTime, while XmlToValue returns DateTime.
                if (Target is GXDLMSCoAPDiagnostic && index == 5 && value is GXStructure { Count: 2 } capture && capture[1] is DateTime time)
                {
                    capture[1] = new GXDateTime(time);
                }
                var expected = wire is DataType.Array or DataType.Structure ? wire : type;
                if (expected == DataType.Array && value is not GXArray || expected == DataType.Structure && value is not GXStructure)
                    throw new FormatException("XML root does not match the COSEM data type.");
            }
            else
            {
                validationError = IntegerInputError(type) ?? defaultError;
                value = type == DataType.None ? text : GXDLMSConverter.ChangeType(text, type, CultureInfo.InvariantCulture);
                if (wire is not DataType.None and not DataType.OctetString && wire != type)
                {
                    validationError = IntegerInputError(wire) ?? defaultError;
                    value = GXDLMSConverter.ChangeType(value, wire, CultureInfo.InvariantCulture);
                }
                validationError = defaultError;
            }
            // Validate using the same object setters as the write path, without changing meter values.
            var unchanged = Comparable(value) == Comparable(ToUI(original, index));
            if (!unchanged && Target is not GXDLMSData && !(Target is GXDLMSRegister && index == 2))
            {
                var clone = Target.Clone();
                var args = new ValueEventArgs(clone, index, 0, null) { Value = value, User = true };
                ((IGXDLMSBase)clone).SetValue(new GXDLMSSettings(), args);
                if (args.Error != ErrorCode.Ok) throw new FormatException(args.Error.ToString());
            }
            // Compare in UI form so OctetString displayed as text can return to its original value.
            if (unchanged) { Target.ClearDirty(index); _drafts.Remove(index); }
            else Target.UpdateDirty(index, value);
            _errors.Remove(index);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            Target.ClearDirty(index);
            _errors[index] = validationError;
        }
        Changed?.Invoke();
    }

    private static string? IntegerInputError(DataType type)
    {
        var range = type switch
        {
            DataType.Int8 => "-128 and 127",
            DataType.UInt8 => "0 and 255",
            DataType.Int16 => "-32768 and 32767",
            DataType.UInt16 => "0 and 65535",
            DataType.Int32 => "-2147483648 and 2147483647",
            DataType.UInt32 => "0 and 4294967295",
            DataType.Int64 => "-9223372036854775808 and 9223372036854775807",
            DataType.UInt64 => "0 and 18446744073709551615",
            _ => null
        };
        return range is null ? null : $"Enter a whole number between {range}.";
    }

    /// <summary>Stages a register scaler, validating its signed-byte base-ten exponent.</summary>
    /// <param name="text">Positive power of ten in invariant notation, such as 0.01.</param>
    public void EditScaler(string text) { _drafts[3] = text; UpdateScalerUnit(); }
    /// <summary>Stages the register unit together with the current scaler draft.</summary>
    /// <param name="unit">COSEM register unit.</param>
    public void EditUnit(Unit unit) { _unit = unit; UpdateScalerUnit(); }
    private void UpdateScalerUnit()
    {
        try
        {
            var register = (GXDLMSRegister)Target;
            var scaler = double.Parse(ScalerText, CultureInfo.InvariantCulture);
            var exponent = Math.Log10(scaler);
            if (!double.IsFinite(exponent) || Math.Abs(exponent - Math.Round(exponent)) > 1e-9 || exponent < sbyte.MinValue || exponent > sbyte.MaxValue)
                throw new FormatException();
            if (scaler == register.Scaler && Unit == register.Unit) Target.ClearDirty(3);
            else Target.UpdateDirty(3, new GXStructure { (sbyte)Math.Round(exponent), new GXEnum((byte)Unit) });
            _errors.Remove(3);
        }
        catch (Exception ex) when (ex is FormatException or OverflowException)
        {
            Target.ClearDirty(3);
            _errors[3] = "Scaler must be a positive power of ten (for example 0.01, 1 or 100).";
        }
        Changed?.Invoke();
    }

    private static string Comparable(object? value) => value is Enum enumValue
        ? Convert.ToInt64(enumValue, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) : Format(value);

    /// <summary>Formats invariant scalars, meter dates, hex buffers and collection XML.</summary>
    /// <param name="value">Value to format.</param>
    /// <returns>Formatted text; null produces an empty string.</returns>
    public static string Format(object? value) => value switch
    {
        null => string.Empty,
        GXDateTime date => date.ToFormatMeterString(CultureInfo.InvariantCulture),
        GXArray or GXStructure => GXDLMSTranslator.ValueToXml(value),
        byte[] bytes => GXDLMSTranslator.ToHex(bytes),
        IFormattable formatted => formatted.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty
    };
}
