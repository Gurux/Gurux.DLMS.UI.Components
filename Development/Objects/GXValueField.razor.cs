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

using Microsoft.AspNetCore.Components;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>A validated inline editor for scalar view-specific command parameters.</summary>
public partial class GXValueField
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public EventCallback<string> OnCommit { get; set; }
    [Parameter] public Type? EnumType { get; set; }
    [Parameter] public string InputKind { get; set; } = "text";
    [Parameter] public Func<string, string?>? Validate { get; set; }
    public string Draft { get; set; } = string.Empty;
    public string? Error { get; private set; }
    public bool Editing { get; private set; }
    private bool _disposed, _accepting;
    private string? _openedValue;
    private readonly string _displayId = $"gx-value-{Guid.NewGuid():N}";
    private readonly string _dialogId = $"gx-value-dialog-{Guid.NewGuid():N}";
    private string DisplayValue => InputKind == "password" && !string.IsNullOrEmpty(Value) ? "••••••••"
        : EnumType?.IsEnum == true && Enum.TryParse(EnumType, Value, out var parsed) ? parsed.ToString() ?? Value : Value;
    public void BeginEdit()
    {
        if (!Enabled || _disposed) return;
        Draft = Value; _openedValue = Value; Error = null; Editing = true;
    }
    public async System.Threading.Tasks.Task ChangeInlineAsync(string value)
    {
        if (!Enabled || _disposed || _accepting) return;
        if (!Editing) BeginEdit();
        Draft = value;
        await AcceptAsync();
    }
    public void CancelEdit() { Editing = false; Error = null; Draft = string.Empty; }
    public async System.Threading.Tasks.Task AcceptAsync()
    {
        if (!Editing || !Enabled || _disposed || _accepting) return;
        if (Value != _openedValue) { Error = "The value changed. Cancel and open the editor again."; return; }
        try
        {
            if (InputKind == "hex") _ = Convert.FromHexString(string.Concat(Draft.Where(character => !char.IsWhiteSpace(character))));
            if (InputKind == "number" && !decimal.TryParse(Draft, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                throw new FormatException("Enter a valid number.");
            if (InputKind == "ip" && !System.Net.IPAddress.TryParse(Draft, out _)) throw new FormatException("Enter a valid IP address.");
            if (InputKind == "datetime" && !DateTime.TryParse(Draft, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _)) throw new FormatException("Enter a valid date and time.");
            Error = Validate?.Invoke(Draft);
            if (Error is not null) return;
            _accepting = true;
            await OnCommit.InvokeAsync(Draft);
            CancelEdit();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
        finally { _accepting = false; }
    }
    public void Dispose() { _disposed = true; CancelEdit(); }
}
