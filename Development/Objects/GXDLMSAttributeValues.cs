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

/// <summary>Formats DLMS object attribute values for display and editing.</summary>
public static class GXDLMSAttributeValues
{
    /// <summary>Gets formatted attribute values, including pending edits, excluding the logical name.</summary>
    /// <param name="target">The object whose attribute values are formatted.</param>
    /// <returns>The formatted values keyed by one-based attribute index.</returns>
    public static IReadOnlyDictionary<int, string?> Get(GXDLMSObject target)
    {
        var result = new Dictionary<int, string?>();
        var values = target.GetValues();
        for (var index = 2; index <= values.Length; ++index)
        {
            var value = values[index - 1];
            if (target.GetDirty(index, out var pending))
            {
                value = pending;
                if (target is GXDLMSRegister && index == 3 && pending is GXStructure scalerUnit && scalerUnit.Count == 2)
                {
                    value = new GXStructure { Math.Pow(10, Convert.ToInt32(scalerUnit[0], CultureInfo.InvariantCulture)), scalerUnit[1] };
                }
            }
            if (value is byte[] bytes && target.GetUIDataType(index) is var ui && ui is not DataType.None and not DataType.OctetString)
            {
                value = GXDLMSClient.ChangeType(bytes, ui);
            }

            result[index] = value is null ? null : GXDLMSObjectEditState.Format(value);
        }
        return result;
    }
}
