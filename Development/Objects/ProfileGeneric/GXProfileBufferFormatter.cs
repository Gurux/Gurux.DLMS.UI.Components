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
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Formats parsed Profile Generic buffer values without editing, decoding or scaling them.</summary>
public sealed class GXProfileBufferFormatter
{
    /// <summary>Keeps display preferences with each profile without retaining discarded profiles.</summary>
    private static readonly ConditionalWeakTable<GXDLMSProfileGeneric, GXProfileBufferFormatter> _formatters = new();
    /// <summary>The profile whose parsed buffer is displayed.</summary>
    private readonly GXDLMSProfileGeneric _profile;

    /// <summary>Creates a formatter for one profile's parsed buffer.</summary>
    /// <param name="profile">The profile whose buffer is displayed.</param>
    private GXProfileBufferFormatter(GXDLMSProfileGeneric profile)
    {
        _profile = profile;
    }

    /// <summary>Gets the formatter and display preferences associated with a profile.</summary>
    /// <param name="profile">The profile whose buffer is displayed.</param>
    /// <returns>The formatter retained for this profile's lifetime.</returns>
    /// <exception cref="ArgumentNullException">The profile is null.</exception>
    public static GXProfileBufferFormatter For(GXDLMSProfileGeneric profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return _formatters.GetValue(profile, value => new GXProfileBufferFormatter(value));
    }

    /// <summary>Gets or sets whether dates retain the meter's timezone instead of converting to local time.</summary>
    /// <remarks>The default is local time, matching the desktop profile display preference.</remarks>
    public bool UseMeterTimeZone { get; set; }

    /// <summary>Formats the current parsed buffer, including a staged buffer when present.</summary>
    /// <returns>Formatted rows, or a diagnostic row if the buffer cannot be enumerated.</returns>
    public IEnumerable<IReadOnlyList<string>> GetRows()
    {
        try
        {
            object? value = _profile.GetDirty(2, out var pending) ? pending : _profile.Buffer;
            if (value is not System.Collections.IEnumerable rows || value is string or byte[])
            {
                return [[FormatCell(value)]];
            }
            return rows.Cast<object?>().Select(row =>
                row is System.Collections.IEnumerable cells && row is not (string or byte[])
                    ? FormatRow(cells.Cast<object?>()) : (IReadOnlyList<string>)[FormatCell(row)]).ToArray();
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            return [[error.Message]];
        }
    }

    /// <summary>Formats one row after the DLMS profile parser has applied capture types and scalers.</summary>
    /// <param name="values">The parsed values in capture-column order.</param>
    /// <returns>The formatted cells without modifying the supplied values.</returns>
    /// <exception cref="ArgumentNullException">The row values are null.</exception>
    public IReadOnlyList<string> FormatRow(IEnumerable<object?> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Select(FormatCell).ToArray();
    }

    /// <summary>Formats meter dates, register descriptors, collections and scalar values for display.</summary>
    /// <param name="value">The parsed value to format.</param>
    /// <returns>The readable date, collection, scalar or hex representation; null produces an empty string.</returns>
    private string FormatCell(object? value)
    {
        if (value is GXDateTime date)
        {
            return UseMeterTimeZone
                ? date.ToFormatMeterString(CultureInfo.CurrentCulture)
                : date.ToFormatString(CultureInfo.CurrentCulture);
        }
        if (value is System.Collections.IEnumerable items && value is not (string or byte[]))
        {
            return "{" + string.Join(", ", items.Cast<object?>().Select(item =>
                item is IFormattable scalar ? scalar.ToString(null, CultureInfo.CurrentCulture) : FormatCell(item))) + "}";
        }
        return value switch
        {
            null => string.Empty,
            byte[] bytes => GXDLMSTranslator.ToHex(bytes),
            IFormattable scalar => scalar.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }
}
