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
using System.Linq.Expressions;
using Gurux.UI.Components.BitEditor;
using Gurux.UI.Components.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Displays supported attribute values as bits without allowing edits.</summary>
public sealed class GXAttributeBits : ComponentBase
{
    /// <summary>Gets or sets the current or pending attribute value.</summary>
    [Parameter] public object? Value { get; set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        object? value = Value;
        if (value is Enum)
            value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), CultureInfo.InvariantCulture);
        switch (value)
        {
            case byte number: Render(builder, number); break;
            case sbyte number: Render(builder, number); break;
            case ushort number: Render(builder, number); break;
            case short number: Render(builder, number); break;
            case uint number: Render(builder, number); break;
            case int number: Render(builder, number); break;
            case ulong number: Render(builder, number); break;
            case long number: Render(builder, number); break;
            case UInt128 number: Render(builder, number); break;
            case Int128 number: Render(builder, number); break;
            case byte[] bytes: Render(builder, bytes); break;
            case GXBitString bits: Render(builder, bits.ToString()); break;
            case string text: Render(builder, text, BitStringMode.Text); break;
        }
    }

    private static void Render<T>(RenderTreeBuilder builder, T value, BitStringMode mode = BitStringMode.BitString)
    {
        builder.OpenComponent<GXBitEditor<T>>(0);
        builder.AddAttribute(1, nameof(GXBitEditor<T>.Value), value);
        builder.AddAttribute(2, nameof(GXBitEditor<T>.ValueExpression), (Expression<Func<T>>)(() => value));
        builder.AddAttribute(3, nameof(GXBitEditor<T>.ReadOnly), true);
        builder.AddAttribute(4, nameof(GXBitEditor<T>.ShowToolbar), false);
        builder.AddAttribute(5, nameof(GXBitEditor<T>.StringMode), mode);
        builder.AddAttribute(6, nameof(GXBitEditor<T>.Disabled), true);
        builder.CloseComponent();
    }
}
