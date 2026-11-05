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

using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>The host must return successfully only after the meter confirms the write.</summary>
/// <param name="Target">COSEM object to write.</param>
/// <param name="AttributeIndex">One-based COSEM attribute index.</param>
/// <param name="Value">Validated value in the meter's representation.</param>
/// <remarks>The host sends the write under its session lock and reports failures by
/// throwing. Completing the callback successfully authorizes the editor to accept the value.</remarks>
public sealed record GXDLMSWriteRequest(GXDLMSObject Target, int AttributeIndex, object Value);
