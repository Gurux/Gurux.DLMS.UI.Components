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

internal static class GXWireValue
{
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
    public static object? Read(GXDLMSObjectEditState edit, int index)
        => Copy(edit.Target.GetDirty(index, out var pending) ? pending : GXDLMSAttributeCodec.Read(edit.Target, index));
    public static string Display(object? value) => value switch
    {
        byte[] { Length: 6 } bytes => string.Join(".", bytes),
        GXStructure structure => structure.All(item => string.IsNullOrEmpty(Display(item))) ? string.Empty : string.Join(" / ", structure.Select(Display)),
        GXArray array => string.Join("; ", array.Select(Display)),
        _ => GXDLMSObjectEditState.Format(value)
    };
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
                    throw new FormatException("Enter a logical name with six numbers.");
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
    public static GXDate ParseDate(string text)
        => DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? new GXDate(date) : new GXDate(text, CultureInfo.InvariantCulture);

    public static string Editable(object? value) => value is GXBitString bits ? bits.ToString()
        : value is GXEnum e ? Convert.ToByte(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) : Display(value);

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
