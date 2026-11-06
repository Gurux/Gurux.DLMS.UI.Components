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

using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Resolves attribute access rights from an association or the object itself.</summary>
public static class GXDLMSObjectAccess
{
    /// <summary>Determines whether attribute access rights permit reading.</summary>
    /// <param name="target">The object containing the attribute.</param>
    /// <param name="index">The one-based attribute index.</param>
    /// <param name="association">The association supplying access rights; object rights are used when no override is set.</param>
    /// <returns>Whether reading is permitted.</returns>
    public static bool CanRead(GXDLMSObject target, int index, GXDLMSAssociationLogicalName? association = null)
        => association?.Version >= 3
            ? (GetAccess3(target, index, association) & AccessMode3.Read) != 0
            : GetAccess(target, index, association)
                is AccessMode.Read or AccessMode.ReadWrite or AccessMode.AuthenticatedRead or AccessMode.AuthenticatedReadWrite;

    /// <summary>Determines whether attribute access rights permit editing.</summary>
    /// <param name="target">The object containing the attribute.</param>
    /// <param name="index">The one-based attribute index.</param>
    /// <param name="association">The association supplying access rights; object rights are used when no override is set.</param>
    /// <returns>Whether writing, and therefore editing, is permitted.</returns>
    public static bool CanEdit(GXDLMSObject target, int index, GXDLMSAssociationLogicalName? association = null)
        => CanWrite(target, index, association);

    /// <summary>Determines whether attribute access rights permit writing.</summary>
    /// <param name="target">The object containing the attribute.</param>
    /// <param name="index">The one-based attribute index.</param>
    /// <param name="association">The association supplying access rights; object rights are used when no override is set.</param>
    /// <returns>Whether writing is permitted.</returns>
    public static bool CanWrite(GXDLMSObject target, int index, GXDLMSAssociationLogicalName? association = null)
        => association?.Version >= 3
            ? (GetAccess3(target, index, association) & AccessMode3.Write) != 0
            : GetAccess(target, index, association)
                is AccessMode.Write or AccessMode.ReadWrite or AccessMode.AuthenticatedWrite or AccessMode.AuthenticatedReadWrite;

    /// <summary>Gets the attribute access mode, preferring association-specific rights when set.</summary>
    /// <param name="target">The object containing the attribute.</param>
    /// <param name="index">The one-based attribute index.</param>
    /// <param name="association">The association supplying access rights; object rights are used when no override is set.</param>
    /// <returns>The attribute access mode.</returns>
    public static AccessMode GetAccess(GXDLMSObject target, int index, GXDLMSAssociationLogicalName? association = null)
        => association?.IsAccessRightSet(target) == true ? association.GetAccess(target, index) : target.GetAccess(index);

    /// <summary>Gets version 3 attribute access flags, preferring association-specific rights when set.</summary>
    /// <param name="target">The object containing the attribute.</param>
    /// <param name="index">The one-based attribute index.</param>
    /// <param name="association">The association supplying access rights; object rights are used when no override is set.</param>
    /// <returns>The version 3 attribute access flags.</returns>
    public static AccessMode3 GetAccess3(GXDLMSObject target, int index, GXDLMSAssociationLogicalName association)
        => association.IsAccessRightSet(target) ? association.GetAccess3(target, index) : target.GetAccess3(index);
}
