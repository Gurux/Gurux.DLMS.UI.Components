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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.UI.Components;
using Gurux.UI.Components.Table;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Displays and edits a COSEM attribute or a nested value within it.</summary>
public partial class GXAttributeField : IDisposable
{
    /// <summary>Gets or sets the shared edit state used to read and stage attribute values.</summary>
    [Parameter, EditorRequired] public GXDLMSObjectEditState Edit { get; set; } = default!;
    /// <summary>Gets or sets the one-based COSEM attribute index.</summary>
    [Parameter]
    public int Index
    {
        get; set;
    }
    /// <summary>Gets or sets a zero-based structure element index when no path is specified.</summary>
    [Parameter]
    public int? Part
    {
        get; set;
    }
    /// <summary>Gets or sets the dot-separated, zero-based element path within the attribute.</summary>
    [Parameter]
    public string? Path
    {
        get; set;
    }
    /// <summary>Gets or sets the label displayed beside the field.</summary>
    [Parameter] public string Label { get; set; } = string.Empty;
    /// <summary>Gets or sets whether editing is enabled, subject to object access rights.</summary>
    [Parameter]
    public bool Enabled
    {
        get; set;
    }
    /// <summary>Gets or sets the enum type used to display and edit numeric or bit-string values.</summary>
    [Parameter]
    public Type? EnumType
    {
        get; set;
    }
    /// <summary>Gets or sets the requested number of input rows.</summary>
    [Parameter] public int Rows { get; set; } = 1;
    /// <summary>Gets or sets the column headings for collection values.</summary>
    [Parameter]
    public string[]? Columns
    {
        get; set;
    }
    /// <summary>Gets or sets the zero-based structure element indices displayed as columns.</summary>
    [Parameter]
    public int[]? ColumnIndices
    {
        get; set;
    }
    /// <summary>Gets or sets the dot-separated element paths displayed as collection columns.</summary>
    [Parameter]
    public string[]? ColumnPaths
    {
        get; set;
    }
    /// <summary>Gets or sets the invariant-culture format strings used for collection columns.</summary>
    [Parameter]
    public string[]? ColumnFormats
    {
        get; set;
    }
    /// <summary>Gets or sets labels for fields in the structured value editor.</summary>
    [Parameter]
    public string[]? EditorLabels
    {
        get; set;
    }
    /// <summary>Gets or sets input kinds for structured editor fields and collection columns.</summary>
    [Parameter]
    public string[]? EditorKinds
    {
        get; set;
    }
    /// <summary>Gets or sets labels keyed by nested editor element path.</summary>
    [Parameter]
    public IReadOnlyDictionary<string, string>? EditorPathLabels
    {
        get; set;
    }
    /// <summary>Gets or sets input kinds keyed by nested editor element path.</summary>
    [Parameter]
    public IReadOnlyDictionary<string, string>? EditorPathKinds
    {
        get; set;
    }
    /// <summary>Gets or sets factories for creating nested values at the specified editor paths.</summary>
    [Parameter]
    public IReadOnlyDictionary<string, Func<object>>? EditorPathFactories
    {
        get; set;
    }
    /// <summary>Gets or sets paths at which the structured editor offers value type selection.</summary>
    [Parameter]
    public string[]? EditorTypePaths
    {
        get; set;
    }
    /// <summary>Gets or sets the factory used to create a collection row before editing it.</summary>
    [Parameter]
    public Func<object>? CreateRow
    {
        get; set;
    }
    /// <summary>Gets or sets the input kind used to display, parse, and edit the value.</summary>
    [Parameter]
    public string? InputKind
    {
        get; set;
    }
    /// <summary>Gets or sets the invariant-culture format string for the displayed value.</summary>
    [Parameter]
    public string? Format
    {
        get; set;
    }
    /// <summary>Gets or sets the cascading access rules for the target object.</summary>
    [CascadingParameter]
    public GXObjectEditorAccess? Access
    {
        get; set;
    }
    private bool CanEditNow => Enabled && !_disposed && (Access is null ? GXDLMSObjectAccess.CanEdit(Edit.Target, Index) : ReferenceEquals(Access.Target, Edit.Target) && Access.CanEdit(Index));
    /// <summary>Gets or sets the uncommitted text being edited.</summary>
    public string Draft { get; set; } = string.Empty;
    /// <summary>Gets the current validation or editing error, or null when no error is present.</summary>
    public string? Error
    {
        get; private set;
    }
    /// <summary>Gets whether an editing session is open.</summary>
    public bool Editing
    {
        get; private set;
    }
    private GXDLMSObjectEditState? _openedEdit, _lastEdit;
    private int _lastIndex;
    private string? _lastPath;
    private int[] Indices => Path is not null ? Path.Split('.').Select(int.Parse).ToArray() : Part.HasValue ? [Part.Value] : [];
    private string CurrentPath => string.Join('.', Indices);
    private object? CurrentValue => At(GXWireValue.Read(Edit, Index), Indices);
    private object? _wireDraft;
    private GXWireValueEditor? _wireEditor;
    private bool _disposed;
    private int? _row;
    private bool _remove;
    private string? _openedFingerprint;
    private static string Fingerprint(object? value) => value?.GetType().FullName + ":" + GXDLMSObjectEditState.Format(value);
    private readonly string _id = $"gx-attribute-{Guid.NewGuid():N}";
    private GXTable<int>? _table;
    private string Display
    {
        get
        {
            try
            {
                var value = CurrentValue;
                if (value is byte[] { Length: 0 })
                {
                    return string.Empty;
                }

                if (InputKind == "password" && !string.IsNullOrEmpty(GXWireValue.Display(value)))
                {
                    return "••••••••";
                }

                if (InputKind == "hex" && value is byte[] hexBytes)
                {
                    return GXDLMSTranslator.ToHex(hexBytes);
                }

                if (Format is not null && value is IFormattable formatted)
                {
                    return formatted.ToString(Format, CultureInfo.InvariantCulture);
                }

                if (value is byte[] bytes && InputKind is "text" or "ascii" or "password")
                {
                    return InputKind == "password" && bytes.Length > 0 ? "••••••••" : System.Text.Encoding.UTF8.GetString(bytes);
                }

                if (InputKind == "ip" && value is byte[] ipBytes)
                {
                    return new System.Net.IPAddress(ipBytes).ToString();
                }

                if (InputKind == "ip" && value is uint address)
                {
                    return new System.Net.IPAddress(BitConverter.GetBytes(address).Reverse().ToArray()).ToString();
                }

                if (InputKind == "scaler")
                {
                    return Math.Pow(10, Convert.ToSByte(value, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                }

                if (EnumType?.IsEnum == true)
                {
                    return Enum.ToObject(EnumType, Convert.ToInt64(value, CultureInfo.InvariantCulture)).ToString() ?? string.Empty;
                }

                return value is GXArray array ? $"{array.Count} items" : GXWireValue.Display(value);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException) { return ex.Message; }
        }
    }
    private bool IsCollection
    {
        get
        {
            try
            {
                return CurrentValue is GXArray;
            }
            catch { return false; }
        }
    }
    private bool IsSimple
    {
        get
        {
            try
            {
                return CurrentValue is not (GXArray or GXStructure) && (Indices.Length != 0 || !Edit.IsXmlAttribute(Index));
            }
            catch { return false; }
        }
    }
    private string InlineValue => Editing ? Draft : DraftText();
    /// <summary>Validates and stages a scalar value entered inline.</summary>
    /// <param name="value">The new text entered in the field.</param>
    /// <returns>A task that completes after validation and any table refresh.</returns>
    public async System.Threading.Tasks.Task ChangeInlineAsync(string value)
    {
        if (!IsSimple || !CanEditNow)
        {
            return;
        }

        if (!Editing)
        {
            BeginEdit();
        }

        Draft = value;
        await AcceptAsync();
    }
    private string Kind => InputKind ?? (CurrentValue is bool ? "bool" : Edit.IsXmlAttribute(Index) && Indices.Length == 0 ? "textarea" : "text");
    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_lastEdit, Edit) || _lastIndex != Index || _lastPath != CurrentPath)
        {
            CancelEdit();
        }

        _lastEdit = Edit;
        _lastIndex = Index;
        _lastPath = CurrentPath;
    }
    private static object? At(object? value, IEnumerable<int> path)
    {
        foreach (var part in path)
        {
            value = ((IList<object>)value!)[part];
        }

        return value;
    }
    private object? Replace(object? root, object? value)
    {
        var path = Indices;
        if (path.Length == 0)
        {
            return value;
        }

        var parent = (IList<object>)At(root, path.Take(path.Length - 1))!;
        parent[path[^1]] = value!;
        return root;
    }
    /// <summary>Starts an editing session with a copy of the current attribute value.</summary>
    public void BeginEdit()
    {
        if (!CanEditNow)
        {
            return;
        }

        _openedEdit = Edit;
        _row = null;
        _remove = false;
        _openedFingerprint = Fingerprint(CurrentValue);
        _wireDraft = GXWireValue.Copy(CurrentValue);
        Draft = DraftText();
        Error = null;
        Editing = true;
    }
    private string DraftText()
    {
        var value = CurrentValue;
        if (InputKind == "logicalname" && value is byte[] name)
        {
            return string.Join(".", name);
        }
        if (InputKind is "scaler" or "ip" || Format is not null)
        {
            return Display;
        }

        if (value is GXBitString flags && EnumType is not null)
        {
            return flags.ToInteger().ToString(CultureInfo.InvariantCulture);
        }

        if (value is byte[] bytes && InputKind is "text" or "ascii" or "password")
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        return Indices.Length != 0 || value is GXBitString ? GXWireValue.Editable(value) : Edit.AttributeText(Index);
    }
    /// <summary>Starts editing a copy of a collection row.</summary>
    /// <param name="row">The zero-based index of the collection row.</param>
    public void BeginRow(int row)
    {
        BeginEdit();
        if (!Editing)
        {
            return;
        }

        _row = row;
        _wireDraft = GXWireValue.Copy(((GXArray)CurrentValue!)[row]);
    }
    /// <summary>Starts editing a new collection row using the row factory or the first existing row.</summary>
    public void BeginAdd()
    {
        BeginEdit();
        if (!Editing)
        {
            return;
        }

        var values = (GXArray)CurrentValue!;
        if (CreateRow is null && values.Count == 0)
        {
            CancelEdit();
            return;
        }
        _row = -1;
        _wireDraft = GXWireValue.Copy(CreateRow?.Invoke() ?? values[0]);
    }
    /// <summary>Opens a confirmation session for removing a collection row.</summary>
    /// <param name="row">The zero-based index of the row to remove.</param>
    public void BeginRemove(int row)
    {
        BeginRow(row);
        _remove = Editing;
    }
    /// <summary>Discards the current editing session and clears its error.</summary>
    public void CancelEdit()
    {
        Editing = false;
        Error = null;
        _openedEdit = null;
        _row = null;
        _wireEditor = null;
    }
    /// <summary>Validates and stages the current draft or collection change in the shared edit state.</summary>
    /// <returns>A task that completes after staging the change and refreshing the table.</returns>
    public async System.Threading.Tasks.Task AcceptAsync()
    {
        if (!Editing || !CanEditNow || !ReferenceEquals(Edit, _openedEdit))
        {
            return;
        }

        try
        {
            if (InputKind == "hex" && _wireDraft is not (GXArray or GXStructure) && !_row.HasValue)
            {
                _ = Convert.FromHexString(string.Concat(Draft.Where(character => !char.IsWhiteSpace(character))));
            }

            if (_openedFingerprint != Fingerprint(CurrentValue))
            {
                Error = "The value changed. Cancel and open the editor again.";
                return;
            }
            if (_wireEditor is not null && !_wireEditor.IsValid)
            {
                Error = "Correct the invalid fields before accepting.";
                return;
            }
            object? value;
            if (_row is int row)
            {
                var values = (GXArray)CurrentValue!;
                if (_remove)
                {
                    values.RemoveAt(row);
                }
                else if (row < 0)
                {
                    values.Add(_wireDraft!);
                }
                else
                {
                    values[row] = _wireDraft!;
                }

                value = Replace(GXWireValue.Read(Edit, Index), values);
            }
            else if (_wireDraft is GXArray or GXStructure)
            {
                value = Replace(GXWireValue.Read(Edit, Index), _wireDraft);
            }
            else if (Indices.Length > 0)
            {
                object partValue;
                if (InputKind == "scaler")
                {
                    var exponent = Math.Log10(double.Parse(Draft, CultureInfo.InvariantCulture));
                    if (!double.IsFinite(exponent) || Math.Abs(exponent - Math.Round(exponent)) > 1e-9 || exponent < sbyte.MinValue || exponent > sbyte.MaxValue)
                    {
                        throw new FormatException("Scaler must be a positive power of ten.");
                    }

                    partValue = (sbyte)Math.Round(exponent);
                }
                else if (CurrentValue is byte[] && InputKind is "text" or "ascii" or "password")
                {
                    partValue = System.Text.Encoding.UTF8.GetBytes(Draft);
                }
                else if (CurrentValue is byte[] && InputKind == "ip")
                {
                    partValue = System.Net.IPAddress.Parse(Draft).GetAddressBytes();
                }
                else if (Format?.StartsWith('X') == true)
                {
                    partValue = Convert.ChangeType(ulong.Parse(Draft, NumberStyles.HexNumber, CultureInfo.InvariantCulture), CurrentValue!.GetType(), CultureInfo.InvariantCulture);
                }
                else
                {
                    partValue = GXWireValue.Parse(Draft, CurrentValue);
                }

                value = Replace(GXWireValue.Read(Edit, Index), partValue);
            }
            else
            {
                var text = CurrentValue is GXBitString bits && EnumType is not null ? new GXBitString(uint.Parse(Draft, CultureInfo.InvariantCulture), (byte)bits.ToString().Length).ToString() : Draft;
                if (InputKind == "datetime" && Draft.Contains('T') && DateTime.TryParse(Draft, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    text = new GXDateTime(date).ToFormatMeterString(CultureInfo.InvariantCulture);
                }

                if (CurrentValue is byte[] && InputKind is "text" or "ascii" or "password")
                {
                    text = GXDLMSTranslator.ToHex(System.Text.Encoding.UTF8.GetBytes(Draft));
                }

                if (Format?.StartsWith('X') == true)
                {
                    text = ulong.Parse(Draft, NumberStyles.HexNumber, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
                }

                if (!GXWireValue.TryAttribute(Edit, Index, text, out var validation))
                {
                    Error = validation;
                    return;
                }
                Edit.EditAttribute(Index, text);
                CancelEdit();
                await RefreshTable();
                return;
            }
            var wireText = GXDLMSObjectEditState.Format(value);
            if (!GXWireValue.TryAttribute(Edit, Index, wireText, out var error))
            {
                Error = error;
                return;
            }
            Edit.EditAttribute(Index, wireText);
            CancelEdit();
            await RefreshTable();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
    }
    private System.Threading.Tasks.Task RefreshTable() => _table?.RefreshDataAsync(false) ?? System.Threading.Tasks.Task.CompletedTask;
    private ValueTask<ItemsProviderResult<int>> ProvideRows(GXItemsProviderRequest request)
    {
        var count = ((GXArray)CurrentValue!).Count;
        return ValueTask.FromResult(new ItemsProviderResult<int>(Enumerable.Range(0, count), count));
    }
    private string[] Cells(int row)
    {
        var value = ((GXArray)CurrentValue!)[row];
        if (ColumnPaths is not null)
        {
            return ColumnPaths.Select((path, column) => FormatCell(At(value, path.Split('.').Select(int.Parse)), EditorKinds?.ElementAtOrDefault(column) ?? InputKind, ColumnFormats?.ElementAtOrDefault(column))).ToArray();
        }

        var cells = value is GXStructure structure ? structure.ToArray() : new[] { value };
        if (ColumnIndices is not null)
        {
            cells = ColumnIndices.Select(index => cells.ElementAtOrDefault(index)).ToArray()!;
        }

        if (Columns?.Length == 1)
        {
            return [string.Join(" / ", cells.Select((cell, column) => FormatCell(cell, EditorKinds?.ElementAtOrDefault(column) ?? InputKind, ColumnFormats?.ElementAtOrDefault(column))))];
        }

        if (Columns is not null && cells.Length != Columns.Length)
        {
            var flattened = Flatten(value).ToArray();
            if (flattened.Length == Columns.Length)
            {
                cells = flattened;
            }
        }
        return cells.Select((cell, column) => FormatCell(cell, EditorKinds?.ElementAtOrDefault(ColumnIndices?.ElementAtOrDefault(column) ?? column) ?? InputKind, ColumnFormats?.ElementAtOrDefault(column))).ToArray();
    }
    private static string FormatCell(object? value, string? kind, string? format)
    {
        if (!string.IsNullOrEmpty(format) && value is IFormattable formatted)
        {
            return formatted.ToString(format, CultureInfo.InvariantCulture);
        }

        if (kind == "password")
        {
            return string.IsNullOrEmpty(GXWireValue.Display(value)) ? string.Empty : "••••••••";
        }

        if (kind is "text" or "ascii" && value is byte[] text)
        {
            return System.Text.Encoding.UTF8.GetString(text);
        }

        if (kind == "hex" && value is byte[] hex)
        {
            return GXDLMSTranslator.ToHex(hex);
        }

        if (kind == "ip" && value is byte[] bytes)
        {
            return new System.Net.IPAddress(bytes).ToString();
        }

        if (kind == "ip" && value is uint address)
        {
            return new System.Net.IPAddress(BitConverter.GetBytes(address).Reverse().ToArray()).ToString();
        }

        if (kind == "datetime" && value is byte[] date)
        {
            return GXDLMSObjectEditState.Format(GXDLMSClient.ChangeType(date, date.Length == 4 ? DataType.Time : date.Length == 5 ? DataType.Date : DataType.DateTime, false));
        }

        return GXWireValue.Display(value);
    }
    private static IEnumerable<object?> Flatten(object? value) => value is GXStructure structure ? structure.SelectMany(Flatten) : [value];
    /// <summary>Prevents further editing and discards the current editing session.</summary>
    public void Dispose()
    {
        _disposed = true;
        CancelEdit();
    }
}
