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
/// <summary>Specifies how profile generic buffer entries are selected for reading.</summary>
public enum GXProfileReadMode
{
    /// <summary>Reads all buffer entries.</summary>
    All,
    /// <summary>Reads entries from the specified number of recent days.</summary>
    LastDays,
    /// <summary>Reads entries within a specified date and time range.</summary>
    Range,
    /// <summary>Reads a specified sequence of buffer entries.</summary>
    Entry
}
/// <summary>Describes a request to read a profile generic buffer.</summary>
/// <param name="Target">The profile generic object to read.</param>
/// <param name="Mode">The method used to select buffer entries.</param>
/// <param name="LastDays">The number of recent days to read in LastDays mode.</param>
/// <param name="From">The start of the date and time range in Range mode.</param>
/// <param name="To">The end of the date and time range in Range mode.</param>
/// <param name="EntryStart">The first buffer entry to read in Entry mode.</param>
/// <param name="EntryCount">The number of buffer entries to read in Entry mode.</param>
public sealed record GXProfileReadRequest(GXDLMSProfileGeneric Target, GXProfileReadMode Mode, uint LastDays, DateTime From, DateTime To, uint EntryStart, uint EntryCount);
