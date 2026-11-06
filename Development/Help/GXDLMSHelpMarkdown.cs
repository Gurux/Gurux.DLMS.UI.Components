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
using System.Text.RegularExpressions;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Selects version-specific sections from the embedded COSEM Markdown resources.</summary>
internal static class GXDLMSHelpMarkdown
{
    private static readonly Regex Counts = new(@"^<!--\s*cosem-attributes:\s*([0-9, ]+)\s*-->$", RegexOptions.CultureInvariant);
    private static readonly Regex Start = new(@"^<!--\s*versions:\s*([0-9, -]+)\s*-->$", RegexOptions.CultureInvariant);
    private static readonly Regex Attribute = new(@"^### ([0-9]+)\. ", RegexOptions.CultureInvariant);
    private static readonly Regex VersionSuffix = new(@"\s+\(versions? [0-9, -]+\)$", RegexOptions.CultureInvariant);

    internal static string? Render(string markdown, byte? version, bool generalTopic)
    {
        string[] lines = markdown.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        int[]? counts = null;
        foreach (string line in lines)
        {
            Match match = Counts.Match(line.Trim());
            if (!match.Success)
            {
                continue;
            }
            if (counts is not null)
            {
                throw new InvalidDataException("COSEM help has more than one attribute-count declaration.");
            }
            counts = match.Groups[1].Value.Split(',').Select(value => int.Parse(value.Trim(), CultureInfo.InvariantCulture)).ToArray();
            if (counts.Length > 256 || counts.Any(count => count < 1))
            {
                throw new InvalidDataException("COSEM help attribute counts must describe valid object versions.");
            }
        }
        if (counts is null && !generalTopic)
        {
            throw new InvalidDataException("COSEM class help is missing its attribute-count declaration.");
        }
        if (version is not null && counts is not null && version.Value >= counts.Length)
        {
            return null;
        }
        int maximumAttribute = version is not null && counts is not null ? counts[version.Value] : int.MaxValue;
        var output = new List<string>();
        var parents = new Stack<bool>();
        bool included = true;
        bool skipAttribute = false;
        bool skipHistory = false;
        foreach (string source in lines)
        {
            string trimmed = source.Trim();
            if (Counts.IsMatch(trimmed))
            {
                continue;
            }
            Match start = Start.Match(trimmed);
            if (start.Success)
            {
                bool matches = Includes(start.Groups[1].Value, version);
                parents.Push(included);
                included = included && matches;
                continue;
            }
            if (trimmed == "<!-- /versions -->")
            {
                if (parents.Count == 0)
                {
                    throw new InvalidDataException("COSEM help has an unmatched version-block ending.");
                }
                included = parents.Pop();
                continue;
            }
            if (!included)
            {
                continue;
            }
            if (source.StartsWith("## ", StringComparison.Ordinal))
            {
                skipAttribute = false;
                skipHistory = version is not null && trimmed == "## Versions";
            }
            Match attribute = Attribute.Match(source);
            if (attribute.Success)
            {
                skipAttribute = int.Parse(attribute.Groups[1].Value, CultureInfo.InvariantCulture) > maximumAttribute;
            }
            if (skipAttribute || skipHistory)
            {
                continue;
            }
            string line = version is not null && attribute.Success ? VersionSuffix.Replace(source, string.Empty) : source;
            if (string.IsNullOrWhiteSpace(line) && (output.Count == 0 || string.IsNullOrWhiteSpace(output[^1])))
            {
                continue;
            }
            output.Add(line);
        }
        if (parents.Count != 0)
        {
            throw new InvalidDataException("COSEM help has an unclosed version block.");
        }
        if (version is null && counts is { Length: > 1 } && !output.Any(line => line.Trim() == "## Versions"))
        {
            output.Add(string.Empty);
            output.Add("## Versions");
            output.Add(string.Empty);
            for (int index = 0; index < counts.Length; ++index)
            {
                output.Add($"- Version {index.ToString(CultureInfo.InvariantCulture)}: attributes 1-{counts[index].ToString(CultureInfo.InvariantCulture)}.");
            }
        }
        string result = string.Join("\r\n", output).TrimEnd() + "\r\n";
        if (version is not null && result.StartsWith("# ", StringComparison.Ordinal))
        {
            int titleEnd = result.IndexOf("\r\n", StringComparison.Ordinal) + 2;
            result = result.Insert(titleEnd, $"\r\nCOSEM version: **{version.Value.ToString(CultureInfo.InvariantCulture)}**\r\n");
        }
        return result;
    }

    private static bool Includes(string ranges, byte? version)
    {
        bool included = version is null;
        foreach (string range in ranges.Split(',', StringSplitOptions.TrimEntries))
        {
            string[] bounds = range.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length is < 1 or > 2 || !byte.TryParse(bounds[0], NumberStyles.None, CultureInfo.InvariantCulture, out byte first))
            {
                throw new InvalidDataException("COSEM help has an invalid version range.");
            }
            byte last = first;
            if (bounds.Length == 2 && (!byte.TryParse(bounds[1], NumberStyles.None, CultureInfo.InvariantCulture, out last) || last < first))
            {
                throw new InvalidDataException("COSEM help has an invalid version range.");
            }
            if (version is not null && version.Value >= first && version.Value <= last)
            {
                included = true;
            }
        }
        return included;
    }
}
