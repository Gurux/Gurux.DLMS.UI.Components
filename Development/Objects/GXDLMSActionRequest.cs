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

/// <summary>The host invokes CreateMessages inside its session lock, sends the messages and awaits the meter response.</summary>
/// <param name="Target">COSEM object whose method is invoked.</param>
/// <param name="MethodIndex">One-based COSEM method index.</param>
/// <param name="CreateMessages">Builds request frames with the host's connected client.
/// Invoke under the session lock so client protocol state is serialized.</param>
/// <remarks>Complete the host callback only after receiving the meter response.
/// Report communication and meter errors by throwing, and store the decoded return value in Result.</remarks>
public sealed record GXDLMSActionRequest(GXDLMSObject Target, int MethodIndex, Func<GXDLMSClient, byte[][]> CreateMessages)
{
    /// <summary>The host sets this to the decoded method return value after receiving the meter response.</summary>
    public object? Result { get; set; }
}
