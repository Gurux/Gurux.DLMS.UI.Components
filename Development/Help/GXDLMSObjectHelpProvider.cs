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

using Gurux.UI.Components;
using Gurux.UI.Components.Enums;
using Gurux.UI.Components.Help;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Provides the embedded English COSEM class documentation through the shared help contract.</summary>
/// <remarks>Filters by the requested COSEM version; requests without a version include all variants. Cultures fall back to English.</remarks>
public sealed class GXDLMSObjectHelpProvider : IGXHelpProvider
{
    /// <summary>Loads Markdown for an owned COSEM topic, or returns null for another topic.</summary>
    /// <param name="request">The class topic and optional object version and culture.</param>
    /// <param name="cancellationToken">Token used to cancel loading, including a cached lookup.</param>
    /// <returns>English help for the requested version, or null for an unknown topic or unsupported version.</returns>
    public async Task<GXHelpContent?> GetHelpAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        string? markdown = await GXDLMSObjectHelpCatalog.LoadAsync(request, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return markdown is null ? null : new GXHelpContent(HelpKind.Markdown, markdown);
    }
}
