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
using Gurux.DLMS.Enums;
using Microsoft.AspNetCore.Components;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Provides shared editing, access checks, and method requests for DLMS object views.</summary>
/// <typeparam name="TObject">The type of DLMS object displayed by the view.</typeparam>
public abstract class GXDLMSObjectViewBase<TObject> : ComponentBase, IDisposable where TObject : GXDLMSObject
{
    /// <summary>Gets or sets the object displayed and edited by the view.</summary>
    [Parameter, EditorRequired] public TObject Target { get; set; } = default!;
    /// <summary>Gets or sets whether editing and method invocation are disabled.</summary>
    [Parameter]
    public bool ReadOnly
    {
        get; set;
    }
    /// <summary>Gets or sets the association used to resolve object access rights.</summary>
    [Parameter]
    public GXDLMSAssociationLogicalName? Association
    {
        get; set;
    }
    /// <summary>Gets or sets the callback notified when the edit state changes.</summary>
    [Parameter]
    public EventCallback<GXDLMSObjectEditState> OnChanged
    {
        get; set;
    }
    /// <summary>Gets or sets the client required to enable method requests.</summary>
    [Parameter]
    public GXDLMSClient? Client
    {
        get; set;
    }
    /// <summary>Gets or sets whether object method requests are enabled.</summary>
    [Parameter] public bool ActionsEnabled { get; set; } = true;
    /// <summary>Gets or sets the callback responsible for executing an object method request.</summary>
    [Parameter]
    public EventCallback<GXDLMSActionRequest> OnActionRequested
    {
        get; set;
    }
    /// <summary>Gets the shared edit state for the current object.</summary>
    protected GXDLMSObjectEditState Edit { get; private set; } = default!;
    /// <summary>Gets or sets the message describing the most recent method request failure.</summary>
    protected string? ActionError
    {
        get; set;
    }
    private bool _invoking;
    private bool _disposed;
    /// <summary>Determines whether the current view state and access rights allow a method request.</summary>
    /// <param name="index">The one-based method index.</param>
    /// <returns>Whether the method can be requested.</returns>
    public bool CanInvokeMethod(int index) => !_disposed && !_invoking && !ReadOnly && ActionsEnabled
        && Client is not null && OnActionRequested.HasDelegate && Target is IGXDLMSBase details
        && index > 0 && index <= details.GetMethodCount()
        && (Association?.Version >= 3
            ? ((Association.IsAccessRightSet(Target) ? Association.GetMethodAccess3(Target, index) : Target.GetMethodAccess3(index)) & MethodAccessMode3.Access) != 0
            : (Association?.IsAccessRightSet(Target) == true ? Association.GetMethodAccess(Target, index) : Target.GetMethodAccess(index)) != MethodAccessMode.NoAccess);

    /// <summary>Requests an object method and updates the edit state with its result or error.</summary>
    /// <param name="index">The one-based method index.</param>
    /// <param name="createMessages">Creates the protocol messages using the client supplied by the request handler.</param>
    /// <returns>A task that completes after the request has been handled.</returns>
    protected async System.Threading.Tasks.Task InvokeMethodAsync(int index, Func<GXDLMSClient, byte[][]> createMessages)
    {
        if (!CanInvokeMethod(index))
        {
            return;
        }

        _invoking = true;
        ActionError = null;
        Edit.SetActionResult(null);
        var target = Target;
        try
        {
            var request = new GXDLMSActionRequest(target, index, client =>
            {
                if (_disposed || !ReferenceEquals(Target, target))
                {
                    throw new InvalidOperationException("The selected object changed before the action started.");
                }

                return createMessages(client);
            });
            await OnActionRequested.InvokeAsync(request);
            if (ReferenceEquals(Target, target) && !_disposed)
            {
                Edit.SetActionResult(request.Result);
                await OnChanged.InvokeAsync(Edit);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            if (ReferenceEquals(Target, target) && !_disposed)
            {
                ActionError = $"Action failed: {ex.Message}";
            }
        }
        finally { _invoking = false; }
    }
    /// <summary>Gets whether the view has been disposed.</summary>
    protected bool IsDisposed => _disposed;
    /// <summary>Determines whether the current view state and access rights allow attribute editing.</summary>
    /// <param name="index">The one-based attribute index.</param>
    /// <returns>Whether the attribute can be edited.</returns>
    public bool CanEditAttribute(int index) => !_disposed && !_invoking && !ReadOnly && GXDLMSObjectAccess.CanEdit(Target, index, Association);
    /// <summary>Updates the pending value of attribute 2 when editing is permitted.</summary>
    /// <param name="text">The text to parse as the new value.</param>
    public void ChangeValue(string text)
    {
        if (CanEditAttribute(2))
        {
            Edit.EditValue(text);
        }
    }
    /// <summary>Updates the pending register scaler when editing attribute 3 is permitted.</summary>
    /// <param name="text">The text to parse as the new scaler.</param>
    public void ChangeScaler(string text)
    {
        if (CanEditAttribute(3))
        {
            Edit.EditScaler(text);
        }
    }
    /// <summary>Updates the pending register unit when editing attribute 3 is permitted.</summary>
    /// <param name="unit">The new unit.</param>
    public void ChangeUnit(Gurux.DLMS.Enums.Unit unit)
    {
        if (CanEditAttribute(3))
        {
            Edit.EditUnit(unit);
        }
    }
    /// <summary>Attaches the edit state for the current target and resets method errors when it changes.</summary>
    protected override void OnParametersSet()
    {
        var next = GXDLMSObjectEditState.For(Target);
        if (ReferenceEquals(next, Edit))
        {
            return;
        }

        if (Edit is not null)
        {
            Edit.Changed -= Changed;
        }

        Edit = next;
        ActionError = null;
        Edit.Changed += Changed;
    }
    private void Changed() => _ = InvokeAsync(async () => { if (_disposed) { return; } await OnChanged.InvokeAsync(Edit); StateHasChanged(); });
    /// <summary>Marks the view as disposed and detaches its edit-state change handler.</summary>
    public void Dispose()
    {
        _disposed = true;
        if (Edit is not null)
        {
            Edit.Changed -= Changed;
        }
    }
}
