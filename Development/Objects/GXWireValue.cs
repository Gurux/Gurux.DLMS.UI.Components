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

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Copies, formats and parses DLMS wire values without flattening their collection types.</summary>
internal static class GXWireValue
{
    /// <summary>Recursively copies DLMS collections and byte buffers while retaining scalar values.</summary>
    /// <param name="value">The input value to convert, format or stage.</param>
    /// <returns>The selected or copied value, or null when the source value is null.</returns>
    public static object? Copy(object? value)
    {
        if (value is GXStructure structure)
        {
            var result = new GXStructure();
            result.AddRange(structure.Select(Copy)!);
            return result;
        }
        if (value is GXArray array)
        {
            var result = new GXArray();
            result.AddRange(array.Select(Copy)!);
            return result;
        }
        return value is byte[] bytes ? bytes.ToArray() : value;
    }
    /// <summary>Copies the pending attribute value, falling back to the confirmed wire representation.</summary>
    /// <param name="edit">The edit state containing the model and its pending attribute values.</param>
    /// <param name="index">The one-based COSEM attribute or method index.</param>
    /// <returns>The selected or copied value, or null when the source value is null.</returns>
    public static object? Read(GXDLMSObjectEditState edit, int index)
        => Copy(edit.Target.GetDirty(index, out var pending) ? pending : GXDLMSAttributeCodec.Read(edit.Target, index));
    /// <summary>Formats logical names, collection cells and scalar wire values for display.</summary>
    /// <param name="value">The input value to convert, format or stage.</param>
    /// <returns>The formatted text for the supplied value.</returns>
    public static string Display(object? value) => value switch
    {
        byte[] { Length: 6 } bytes => string.Join(".", bytes),
        GXStructure structure => structure.All(item => string.IsNullOrEmpty(Display(item))) ? string.Empty : string.Join(" / ", structure.Select(Display)),
        GXArray array => string.Join("; ", array.Select(Display)),
        _ => GXDLMSObjectEditState.Format(value)
    };
    /// <summary>Parses invariant input using the original value&apos;s wire or CLR type.</summary>
    /// <param name="text">The user input in the editor&apos;s invariant text representation.</param>
    /// <param name="original">The original value whose type determines how input is parsed.</param>
    /// <returns>The parsed value, retaining the required DLMS or CLR data type.</returns>
    public static object Parse(string text, object? original)
    {
        if (original is GXEnum)
        {
            return new GXEnum(byte.Parse(text, CultureInfo.InvariantCulture));
        }

        if (original is GXBitString)
        {
            return new GXBitString(text);
        }

        if (original is byte[] bytes)
        {
            if (bytes.Length == 6 && text.Contains('.'))
            {
                var parts = text.Split('.');
                if (parts.Length != 6)
                {
                    throw new FormatException(Properties.Resources.EnterALogicalNameWithSixNumbers);
                }

                return parts.Select(part => byte.Parse(part, NumberStyles.None, CultureInfo.InvariantCulture)).ToArray();
            }
            return Convert.FromHexString(string.Concat(text.Where(character => !char.IsWhiteSpace(character))));
        }
        if (original is GXTime)
        {
            return new GXTime(text, CultureInfo.InvariantCulture);
        }

        if (original is GXDate)
        {
            return ParseDate(text);
        }

        if (original is GXDateTime)
        {
            return new GXDateTime(text, CultureInfo.InvariantCulture);
        }

        if (original is Enum)
        {
            return Enum.Parse(original.GetType(), text);
        }

        if (original is not null && original is not string)
        {
            return Convert.ChangeType(text, original.GetType(), CultureInfo.InvariantCulture)!;
        }

        return text;
    }
    /// <summary>Parses an ISO date or a meter date with invariant formatting.</summary>
    /// <param name="text">The user input in the editor&apos;s invariant text representation.</param>
    /// <returns>The parsed value in the representation required by the method.</returns>
    public static GXDate ParseDate(string text)
        => DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? new GXDate(date) : new GXDate(text, CultureInfo.InvariantCulture);

    /// <summary>Formats bit strings and enum values for an editable scalar input.</summary>
    /// <param name="value">The input value to convert, format or stage.</param>
    /// <returns>The formatted text for the supplied value.</returns>
    public static string Editable(object? value) => value is GXBitString bits ? bits.ToString()
        : value is GXEnum e ? Convert.ToByte(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) : Display(value);

    /// <summary>Validates attribute input against an isolated model clone without staging changes on the source.</summary>
    /// <param name="source">The edit state containing the model and its pending attribute values.</param>
    /// <param name="index">The one-based COSEM attribute or method index.</param>
    /// <param name="text">The user input in the editor&apos;s invariant text representation.</param>
    /// <param name="error">Receives the validation error, or null when the input is valid.</param>
    /// <returns>True when validation or lookup succeeds; otherwise, false.</returns>
    public static bool TryAttribute(GXDLMSObjectEditState source, int index, string text, out string? error)
    {
        try
        {
            GXDLMSObject clone;
            if (source.Target is GXDLMSData data)
            {
                var copy = new GXDLMSData(data.LogicalName ?? "0.0.0.0.0.0") { Value = Copy(data.Value), Version = data.Version };
                copy.SetDataType(2, data.GetDataType(2));
                copy.SetUIDataType(2, data.GetUIDataType(2));
                clone = copy;
            }
            else
            {
                clone = source.Target.Clone();
            }

            var trial = GXDLMSObjectEditState.For(clone);
            trial.EditAttribute(index, text);
            error = trial.Error(index);
            return error is null;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { error = ex.Message; return false; }
    }
}
