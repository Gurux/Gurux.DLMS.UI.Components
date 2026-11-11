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
using Gurux.DLMS.Objects;
using Gurux.UI.Components.Help;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Maps COSEM classes to localized official Gurux articles.</summary>
public static class GXDLMSObjectHelpCatalog
{
    private static readonly HashSet<string> Topics = GXDLMSClient.GetObjectTypes()
        .Where(type => !type.IsDefined(typeof(ObsoleteAttribute), false))
        .Select(type => "cosem." + type.Name).ToHashSet(StringComparer.Ordinal);
    private static readonly Lazy<GXDLMSHelpArticleReader> Reader = new(() =>
        new GXDLMSHelpArticleReader(new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })));

    /// <summary>Gets a stable class topic, including the nearest supported base class for custom objects.</summary>
    public static string GetTopic(GXDLMSObject target)
    {
        ArgumentNullException.ThrowIfNull(target);
        for (Type? type = target.GetType(); type != null && type != typeof(GXDLMSObject); type = type.BaseType)
        {
            var topic = "cosem." + type.Name;
            if (Topics.Contains(topic))
            {
                return topic;
            }
        }
        return "cosem.Object";
    }

    /// <summary>Creates a request retaining the object's version and current UI language.</summary>
    public static GXHelpRequest GetRequest(GXDLMSObject target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return new(GetTopic(target), target.Version.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentUICulture.Name);
    }

    /// <summary>Returns a fixed official article URL for a known COSEM topic, or null for an unknown topic.</summary>
    /// <remarks>Finnish uses /fi/. Other UI languages use the English article.</remarks>
    public static Uri? GetAddress(GXHelpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!Topics.Contains(request.Topic))
        {
            return null;
        }
        var culture = request.Culture ?? CultureInfo.CurrentUICulture.Name;
        var finnish = culture.Equals("fi", StringComparison.OrdinalIgnoreCase) || culture.StartsWith("fi-", StringComparison.OrdinalIgnoreCase);
        return new Uri("https://www.gurux.fi/" + (finnish ? "fi/" : "") + "Gurux.DLMS.Objects." + request.Topic[6..]);
    }

    /// <summary>Loads HTML for browser article extraction and sanitization.</summary>
    public static Task<string?> LoadHtmlAsync(string topic, CancellationToken cancellationToken = default)
        => LoadHtmlAsync(new GXHelpRequest(topic), cancellationToken);

    /// <summary>Loads localized online help. Official articles describe their own version variants.</summary>
    /// <remarks>Version remains context metadata; the online article is not filtered by object version.</remarks>
    public static Task<string?> LoadHtmlAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
        => Reader.Value.LoadHtmlAsync(request, cancellationToken);
}
