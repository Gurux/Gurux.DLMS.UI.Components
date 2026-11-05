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

public abstract class GXDLMSObjectViewBase<TObject> : ComponentBase, IDisposable where TObject : GXDLMSObject
{
    [Parameter, EditorRequired] public TObject Target { get; set; } = default!;
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public GXDLMSAssociationLogicalName? Association { get; set; }
    [Parameter] public EventCallback<GXDLMSObjectEditState> OnChanged { get; set; }
    [Parameter] public GXDLMSClient? Client { get; set; }
    [Parameter] public bool ActionsEnabled { get; set; } = true;
    [Parameter] public EventCallback<GXDLMSActionRequest> OnActionRequested { get; set; }
    protected GXDLMSObjectEditState Edit { get; private set; } = default!;
    protected string? ActionError { get; set; }
    private bool _invoking;
    private bool _disposed;
    public bool CanInvokeMethod(int index) => !_disposed && !_invoking && !ReadOnly && ActionsEnabled
        && Client is not null && OnActionRequested.HasDelegate && Target is IGXDLMSBase details
        && index > 0 && index <= details.GetMethodCount()
        && (Association?.Version >= 3
            ? ((Association.IsAccessRightSet(Target) ? Association.GetMethodAccess3(Target, index) : Target.GetMethodAccess3(index)) & MethodAccessMode3.Access) != 0
            : (Association?.IsAccessRightSet(Target) == true ? Association.GetMethodAccess(Target, index) : Target.GetMethodAccess(index)) != MethodAccessMode.NoAccess);

    protected async System.Threading.Tasks.Task InvokeMethodAsync(int index, Func<GXDLMSClient, byte[][]> createMessages)
    {
        if (!CanInvokeMethod(index)) return;
        _invoking = true;
        ActionError = null;
        Edit.SetActionResult(null);
        var target = Target;
        try
        {
            var request = new GXDLMSActionRequest(target, index, client =>
            {
                if (_disposed || !ReferenceEquals(Target, target)) throw new InvalidOperationException("The selected object changed before the action started.");
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
            if (ReferenceEquals(Target, target) && !_disposed) ActionError = $"Action failed: {ex.Message}";
        }
        finally { _invoking = false; }
    }
    protected bool IsDisposed => _disposed;
    public bool CanEditAttribute(int index) => !_disposed && !_invoking && !ReadOnly && GXDLMSObjectAccess.CanEdit(Target, index, Association);
    public void ChangeValue(string text) { if (CanEditAttribute(2)) Edit.EditValue(text); }
    public void ChangeScaler(string text) { if (CanEditAttribute(3)) Edit.EditScaler(text); }
    public void ChangeUnit(Gurux.DLMS.Enums.Unit unit) { if (CanEditAttribute(3)) Edit.EditUnit(unit); }
    protected override void OnParametersSet()
    {
        var next = GXDLMSObjectEditState.For(Target);
        if (ReferenceEquals(next, Edit)) return;
        if (Edit is not null) Edit.Changed -= Changed;
        Edit = next;
        ActionError = null;
        Edit.Changed += Changed;
    }
    private void Changed() => _ = InvokeAsync(async () => { if (_disposed) return; await OnChanged.InvokeAsync(Edit); StateHasChanged(); });
    public void Dispose() { _disposed = true; if (Edit is not null) Edit.Changed -= Changed; }
}
