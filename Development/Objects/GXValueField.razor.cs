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
    /// <summary>Gets or sets the label displayed beside the field.</summary>
    [Parameter] public string Label { get; set; } = string.Empty;
    /// <summary>Gets or sets the committed text displayed by the field.</summary>
    [Parameter] public string Value { get; set; } = string.Empty;
    /// <summary>Gets or sets whether the field can be edited.</summary>
    [Parameter] public bool Enabled { get; set; } = true;
    /// <summary>Gets or sets the callback invoked with a successfully validated draft.</summary>
    [Parameter]
    public EventCallback<string> OnCommit
    {
        get; set;
    }
    /// <summary>Gets or sets the enum type used to display and select values.</summary>
    [Parameter]
    public Type? EnumType
    {
        get; set;
    }
    /// <summary>Gets or sets the input kind used for rendering and built-in validation.</summary>
    [Parameter] public string InputKind { get; set; } = "text";
    /// <summary>Gets or sets an optional validator that returns an error message, or null for a valid draft.</summary>
    [Parameter]
    public Func<string, string?>? Validate
    {
        get; set;
    }
    /// <summary>Gets or sets the uncommitted text being edited.</summary>
    public string Draft { get; set; } = string.Empty;
    /// <summary>Gets the current validation or editing error, or null when no error is present.</summary>
    public string? Error
    {
        get; private set;
    }
    /// <summary>Gets whether an editing session is open.</summary>
    public bool Editing
    {
        get; private set;
    }
    private bool _disposed, _accepting;
    private string? _openedValue;
    private readonly string _displayId = $"gx-value-{Guid.NewGuid():N}";
    private readonly string _dialogId = $"gx-value-dialog-{Guid.NewGuid():N}";
    private string DisplayValue => InputKind == "password" && !string.IsNullOrEmpty(Value) ? "••••••••"
        : EnumType?.IsEnum == true && Enum.TryParse(EnumType, Value, out var parsed) ? parsed.ToString() ?? Value : Value;
    /// <summary>Starts editing a copy of the current value.</summary>
    public void BeginEdit()
    {
        if (!Enabled || _disposed)
        {
            return;
        }

        Draft = Value;
        _openedValue = Value;
        Error = null;
        Editing = true;
    }
    /// <summary>Validates and commits text entered inline.</summary>
    /// <param name="value">The new text entered in the field.</param>
    /// <returns>A task that completes after the commit callback finishes.</returns>
    public async System.Threading.Tasks.Task ChangeInlineAsync(string value)
    {
        if (!Enabled || _disposed || _accepting)
        {
            return;
        }

        if (!Editing)
        {
            BeginEdit();
        }

        Draft = value;
        await AcceptAsync();
    }
    /// <summary>Discards the current editing session and clears its error.</summary>
    public void CancelEdit()
    {
        Editing = false;
        Error = null;
        Draft = string.Empty;
    }
    /// <summary>Validates the current draft and invokes the commit callback when it is valid.</summary>
    /// <returns>A task that completes after validation and any commit callback.</returns>
    public async System.Threading.Tasks.Task AcceptAsync()
    {
        if (!Editing || !Enabled || _disposed || _accepting)
        {
            return;
        }

        if (Value != _openedValue)
        {
            Error = "The value changed. Cancel and open the editor again.";
            return;
        }
        try
        {
            if (InputKind == "hex")
            {
                _ = Convert.FromHexString(string.Concat(Draft.Where(character => !char.IsWhiteSpace(character))));
            }

            if (InputKind == "number" && !decimal.TryParse(Draft, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
            {
                throw new FormatException("Enter a valid number.");
            }

            if (InputKind == "ip" && !System.Net.IPAddress.TryParse(Draft, out _))
            {
                throw new FormatException("Enter a valid IP address.");
            }

            if (InputKind == "datetime" && !DateTime.TryParse(Draft, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
            {
                throw new FormatException("Enter a valid date and time.");
            }

            Error = Validate?.Invoke(Draft);
            if (Error is not null)
            {
                return;
            }

            _accepting = true;
            await OnCommit.InvokeAsync(Draft);
            CancelEdit();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
        finally { _accepting = false; }
    }
    /// <summary>Prevents further editing and discards the current editing session.</summary>
    public void Dispose()
    {
        _disposed = true;
        CancelEdit();
    }
}
