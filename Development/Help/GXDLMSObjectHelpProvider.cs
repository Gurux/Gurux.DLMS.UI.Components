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

/// <summary>Provides online Gurux COSEM articles without the surrounding website.</summary>
public sealed class GXDLMSObjectHelpProvider : IGXHelpProvider
{
    private readonly bool _html;
    private readonly Func<GXHelpRequest, CancellationToken, Task<string?>> _load;

    /// <summary>Uses the shared online article reader and its language-specific cache.</summary>
    public GXDLMSObjectHelpProvider() : this(GXDLMSObjectHelpCatalog.LoadHtmlAsync)
    {
        _html = true;
    }

    /// <summary>Uses a host-owned Markdown loader, without HTML conversion.</summary>
    public GXDLMSObjectHelpProvider(Func<GXHelpRequest, CancellationToken, Task<string?>> load)
    {
        _load = load ?? throw new ArgumentNullException(nameof(load));
    }

    /// <inheritdoc />
    public async Task<GXHelpContent?> GetHelpAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        if (GXDLMSObjectHelpCatalog.GetAddress(request) == null)
        {
            return null;
        }
        var html = await _load(request, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        if (!_html)
        {
            // Existing host delegates supply Markdown directly; no HTML conversion is involved.
            return html == null ? null : new GXHelpContent(HelpKind.Markdown, html);
        }
        return html == null ? null : new GXHelpContent(HelpKind.Html, html)
        {
            BaseAddress = GXDLMSObjectHelpCatalog.GetAddress(request)!.AbsoluteUri,
            ArticleSelector = "article.node--type-book .field--name-body, article .field--name-body"
        };
    }
}
