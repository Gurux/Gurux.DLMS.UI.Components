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

using System.Collections.Concurrent;
using System.Globalization;
using Gurux.UI.Components.Help;
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Loads the library's embedded English COSEM help without a server or file-system dependency.</summary>
public static class GXDLMSObjectHelpCatalog
{
    private const string ResourcePrefix = "Gurux.DLMS.UI.Components.Help.Topics.";
    private const string TopicPrefix = "cosem.";
    private static readonly IReadOnlyDictionary<string, string> Resources = typeof(GXDLMSObjectHelpCatalog).Assembly
        .GetManifestResourceNames()
        .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal) && name.EndsWith(".md", StringComparison.Ordinal))
        .ToDictionary(name => TopicPrefix + name[ResourcePrefix.Length..^3], StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, string> RawCache = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<(string Topic, byte? Version), string> RenderedCache = new();

    /// <summary>Gets a stable class topic, falling back to general object help for an unknown class.</summary>
    public static string GetTopic(GXDLMSObject target)
    {
        ArgumentNullException.ThrowIfNull(target);
        for (Type? type = target.GetType(); type is not null && type != typeof(GXDLMSObject); type = type.BaseType)
        {
            string topic = TopicPrefix + type.Name;
            if (Resources.ContainsKey(topic))
            {
                return topic;
            }
        }
        return TopicPrefix + "Object";
    }

    /// <summary>Creates a shared help request for this object's class and current COSEM version.</summary>
    /// <param name="target">The selected COSEM object.</param>
    /// <returns>A stable class topic and an invariant version string; culture uses the provider's default.</returns>
    public static GXHelpRequest GetRequest(GXDLMSObject target)
    {
        return new GXHelpRequest(GetTopic(target), target.Version.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>Loads and caches Markdown on demand; returns null for an unrecognised topic.</summary>
    /// <param name="topic">A topic returned by <see cref="GetTopic"/> or a host help context.</param>
    /// <param name="cancellationToken">Cancels this read, including reads already present in the cache.</param>
    /// <returns>The complete class Markdown, with version alternatives and no authoring directives, or null for an unknown topic.</returns>
    public static Task<string?> LoadAsync(string topic, CancellationToken cancellationToken = default)
    {
        return LoadVersionAsync(topic, null, cancellationToken);
    }

    /// <summary>Loads only the attributes and descriptions applicable to a requested COSEM version.</summary>
    /// <param name="request">A class topic with an optional decimal byte version; null version requests the complete class help.</param>
    /// <param name="cancellationToken">Token used to cancel loading, including a cached lookup.</param>
    /// <returns>English Markdown, or null for an unknown topic or invalid/unsupported version.</returns>
    public static Task<string?> LoadAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        byte? version = null;
        if (request.Version is not null)
        {
            if (!byte.TryParse(request.Version, NumberStyles.None, CultureInfo.InvariantCulture, out byte parsed))
            {
                return Task.FromResult<string?>(null);
            }
            version = parsed;
        }
        return LoadVersionAsync(request.Topic, version, cancellationToken);
    }

    private static async Task<string?> LoadVersionAsync(string topic, byte? version, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(topic);
        cancellationToken.ThrowIfCancellationRequested();
        var key = (topic, version);
        if (RenderedCache.TryGetValue(key, out string? cached))
        {
            return cached;
        }
        string? raw = await LoadRawAsync(topic, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        if (raw is null)
        {
            return null;
        }
        string? rendered = GXDLMSHelpMarkdown.Render(raw, version, topic == "cosem.Object");
        cancellationToken.ThrowIfCancellationRequested();
        return rendered is null ? null : RenderedCache.GetOrAdd(key, rendered);
    }

    private static async Task<string?> LoadRawAsync(string topic, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(topic);
        cancellationToken.ThrowIfCancellationRequested();
        if (RawCache.TryGetValue(topic, out string? cached))
        {
            return cached;
        }
        if (!Resources.TryGetValue(topic, out string? resource))
        {
            return null;
        }
        using Stream stream = typeof(GXDLMSObjectHelpCatalog).Assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"Missing COSEM help resource: {resource}");
        using var reader = new StreamReader(stream);
        string markdown = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        return RawCache.GetOrAdd(topic, markdown);
    }
}
