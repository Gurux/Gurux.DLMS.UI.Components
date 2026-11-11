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

using Gurux.UI.Components.Help;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Retrieves official COSEM HTML documents for the isolated help viewer.</summary>
/// <remarks>The host owns the supplied HTTP client. Disable automatic redirects; only official article URLs are requested.</remarks>
public sealed class GXDLMSHelpArticleReader : IDisposable
{
    private readonly GXHelpArticleReader _reader;

    /// <summary>Creates an article reader using the caller-owned HTTP client.</summary>
    public GXDLMSHelpArticleReader(HttpClient http)
    {
        _reader = new GXHelpArticleReader(http, IsOfficialArticle);
    }

    /// <summary>Loads the requested language, falling back to English when a translation is absent.</summary>
    public async Task<string?> LoadHtmlAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var address = GXDLMSObjectHelpCatalog.GetAddress(request);
        if (address == null)
        {
            return null;
        }
        var fallbackAddress = address.AbsolutePath.StartsWith("/fi/", StringComparison.Ordinal)
            ? GXDLMSObjectHelpCatalog.GetAddress(request with { Culture = "en" })
            : null;
        return await _reader.LoadHtmlAsync(address, fallbackAddress, cancellationToken).ConfigureAwait(false);
    }

    private static bool IsOfficialArticle(Uri address)
    {
        return address.IsAbsoluteUri
            && address.Scheme == Uri.UriSchemeHttps
            && address.Host.Equals("www.gurux.fi", StringComparison.OrdinalIgnoreCase)
            && address.IsDefaultPort
            && address.UserInfo.Length == 0
            && (address.AbsolutePath.StartsWith("/Gurux.DLMS.Objects.", StringComparison.Ordinal)
                || address.AbsolutePath.StartsWith("/fi/Gurux.DLMS.Objects.", StringComparison.Ordinal));
    }

    /// <summary>Releases reader resources without disposing the caller-owned HTTP client.</summary>
    public void Dispose()
    {
        _reader.Dispose();
    }
}
