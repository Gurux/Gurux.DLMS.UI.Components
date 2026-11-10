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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Gurux.UI.Components.Help;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Retrieves official COSEM article bodies and converts them to inert Markdown.</summary>
/// <remarks>The host owns the supplied HTTP client. Disable automatic redirects; only official article URLs are requested.</remarks>
public sealed class GXDLMSHelpArticleReader(HttpClient http)
{
    private const int MaximumBytes = 2 * 1024 * 1024;
    private readonly ConcurrentDictionary<Uri, (DateTimeOffset Expires, string Markdown)> _cache = new();

    /// <summary>Loads the requested language, falling back to English when a translation is absent.</summary>
    public async Task<string?> LoadAsync(GXHelpRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var address = GXDLMSObjectHelpCatalog.GetAddress(request);
        if (address == null)
        {
            return null;
        }
        if (_cache.TryGetValue(address, out var cached) && cached.Expires > DateTimeOffset.UtcNow)
        {
            return cached.Markdown;
        }
        var markdown = await ReadAsync(address, cancellationToken).ConfigureAwait(false);
        if (markdown == null && address.AbsolutePath.StartsWith("/fi/", StringComparison.Ordinal))
        {
            markdown = await LoadAsync(request with { Culture = "en" }, cancellationToken).ConfigureAwait(false);
        }
        if (markdown != null)
        {
            _cache[address] = (DateTimeOffset.UtcNow.AddHours(1), markdown);
        }
        return markdown;
    }

    private async Task<string?> ReadAsync(Uri address, CancellationToken token)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(token);
        deadline.CancelAfter(TimeSpan.FromSeconds(15));
        using var response = await http.GetAsync(address, HttpCompletionOption.ResponseHeadersRead, deadline.Token).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        if (response.Content.Headers.ContentLength > MaximumBytes)
        {
            throw new InvalidDataException("The COSEM help article is too large.");
        }
        await using var stream = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false);
        using var buffer = new MemoryStream();
        var bytes = new byte[8192];
        int count;
        while ((count = await stream.ReadAsync(bytes, deadline.Token).ConfigureAwait(false)) != 0)
        {
            if (buffer.Length + count > MaximumBytes)
            {
                throw new InvalidDataException("The COSEM help article is too large.");
            }
            await buffer.WriteAsync(bytes.AsMemory(0, count), deadline.Token).ConfigureAwait(false);
        }
        using var document = await new HtmlParser().ParseDocumentAsync(Encoding.UTF8.GetString(buffer.ToArray()), deadline.Token).ConfigureAwait(false);
        var article = document.QuerySelector("article.node--type-book .field--name-body, article .field--name-body");
        if (article == null)
        {
            return null;
        }
        var markdown = Render(article, address, 0).Trim();
        return markdown.Length == 0 ? null : markdown;
    }

    private static string Render(INode node, Uri address, int depth)
    {
        if (depth > 128)
        {
            throw new InvalidDataException("The COSEM help article is too deeply nested.");
        }
        if (node is IText text)
        {
            return Regex.Replace(text.Data, @"\s+", " ");
        }
        if (node is not IElement element || element.LocalName is "script" or "style" or "form" or "iframe" or "object" or "embed" or "svg" or "math" or "nav" or "header" or "footer")
        {
            return "";
        }
        string Children() => string.Concat(element.ChildNodes.Select(child => Render(child, address, depth + 1)));
        var tag = element.LocalName;
        if (tag is "h1" or "h2" or "h3" or "h4" or "h5" or "h6")
        {
            return "\n\n" + new string('#', tag[1] - '0') + " " + Children().Trim() + "\n\n";
        }
        if (tag == "pre")
        {
            return "\n\n```\n" + element.TextContent.Replace("\r", "").Replace("```", "`\u200B``") + "\n```\n\n";
        }
        if (tag == "table")
        {
            var rows = element.QuerySelectorAll("tr").Select(row => row.Children.Where(cell => cell.LocalName is "td" or "th")
                .Select(cell => Render(cell, address, depth + 1).Replace("\r", "").Replace("\n", " ").Replace("|", @"\|").Trim()).ToArray()).Where(row => row.Length != 0).ToArray();
            if (rows.Length == 0)
            {
                return "";
            }
            var columns = rows.Max(row => row.Length);
            string Row(string[] cells) => "| " + string.Join(" | ", Enumerable.Range(0, columns).Select(index => index < cells.Length ? cells[index] : "")) + " |\n";
            return "\n\n" + Row(rows[0]) + Row(Enumerable.Repeat("---", columns).ToArray()) + string.Concat(rows.Skip(1).Select(Row)) + "\n";
        }
        if (tag is "a" or "img")
        {
            var target = element.GetAttribute(tag == "a" ? "href" : "src");
            var label = tag == "a" ? Children() : element.GetAttribute("alt") ?? "";
            if (!string.IsNullOrWhiteSpace(target) && Uri.TryCreate(address, target, out var uri) && uri.Scheme is "https" or "http")
            {
                return (tag == "img" ? "!" : "") + "[" + label + "](" + uri.AbsoluteUri.Replace("<", "%3C").Replace(">", "%3E").Replace("(", "%28").Replace(")", "%29") + ")";
            }
            return label;
        }
        var content = Children();
        return tag switch
        {
            "strong" or "b" => "**" + content + "**",
            "em" or "i" => "*" + content + "*",
            "code" => "`" + content.Replace("`", "&#96;") + "`",
            "br" => "  \n",
            "li" => "\n- " + content.Trim().Replace("\n", "\n  ") + "\n",
            "p" or "div" or "section" or "ul" or "ol" or "dl" or "dt" or "dd" or "blockquote" => "\n\n" + content.Trim() + "\n\n",
            _ => content
        };
    }
}
