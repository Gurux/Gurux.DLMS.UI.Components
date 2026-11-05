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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Objects.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Edits account reference lists using typed COSEM drafts, preserving confirmed meter values.</summary>
public partial class GXAccountTable : IDisposable
{
    [Parameter, EditorRequired] public GXDLMSObjectEditState Edit { get; set; } = default!;
    [Parameter] public int Index { get; set; }
    [Parameter] public bool Enabled { get; set; }
    [CascadingParameter] public GXObjectEditorAccess? Access { get; set; }
    private bool _disposed;
    private bool CanEditNow => !_disposed && Enabled && (Access is null ? GXDLMSObjectAccess.CanEdit(Edit.Target, Index) : ReferenceEquals(Access.Target, Edit.Target) && Access.CanEdit(Index));
    public int SelectedRow { get; private set; } = -1;
    public bool Editing { get; private set; }
    public string? Error { get; private set; }
    public string Reference { get; set; } = string.Empty;
    public string ChargeReference { get; set; } = string.Empty;
    public CreditCollectionConfiguration Configuration { get; set; }
    public string TokenProportionText { get; set; } = "0";
    private int _editRow = -1;
    private bool _removing;
    private ValueTask<Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<int>> ProvideRows(Gurux.UI.Components.GXItemsProviderRequest request)
        => ValueTask.FromResult(new Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<int>(Enumerable.Range(0, Rows.Count), Rows.Count));
    private GXDLMSObjectEditState? _lastEdit;
    private int _lastIndex;
    private readonly string _referenceListId = $"account-references-{Guid.NewGuid():N}";
    private readonly string _chargeListId = $"account-charges-{Guid.NewGuid():N}";
    private string Title => Index switch { 9 => "Credit reference", 10 => "Charge Reference", 11 => "Credit Charge", _ => "Token Gateway" };
    private string[] Headers => Index switch { 9 or 10 => ["Name"], 11 => ["Credit", "Charge", "Configuration"], _ => ["Credit Reference", "Token proportion"] };
    private GXArray Rows
    {
        get
        {
            var rows = new GXArray();
            rows.AddRange((GXArray)(Edit.Target.GetDirty(Index, out var pending) ? pending : GXDLMSAttributeCodec.Read(Edit.Target, Index))!);
            return rows;
        }
    }
    private bool HasSelection => SelectedRow >= 0 && SelectedRow < Rows.Count;
    private IEnumerable<string> ReferenceOptions => ObjectNames(Index == 10 ? ObjectType.Charge : ObjectType.Credit);
    private IEnumerable<string> ChargeOptions => ObjectNames(ObjectType.Charge);
    private IEnumerable<string> ObjectNames(ObjectType type) => Edit.Target.Parent?.GetObjects(type).Select(item => item.LogicalName) ?? [];
    private static string LogicalName(object value) => value is byte[] bytes ? string.Join(".", bytes) : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    private static byte[] LogicalNameBytes(string text)
    {
        var parts = text.Split('.');
        if (parts.Length != 6 || parts.Any(part => !byte.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out _)))
            throw new FormatException("Enter a logical name with six numbers between 0 and 255.");
        return parts.Select(part => byte.Parse(part, CultureInfo.InvariantCulture)).ToArray();
    }
    private IEnumerable<string> Cells(int row)
    {
        var value = Rows[row];
        if (Index <= 10) return [LogicalName(value)];
        var cells = (GXStructure)value;
        return Index == 11
            ? [LogicalName(cells[0]), LogicalName(cells[1]), ((CreditCollectionConfiguration)Convert.ToByte(cells[2], CultureInfo.InvariantCulture)).ToString()]
            : [LogicalName(cells[0]), Convert.ToString(cells[1], CultureInfo.InvariantCulture) ?? "0"];
    }
    protected override void OnParametersSet()
    {
        if (Index is < 9 or > 12) throw new ArgumentOutOfRangeException(nameof(Index));
        if (ReferenceEquals(_lastEdit, Edit) && _lastIndex == Index) return;
        _lastEdit = Edit; _lastIndex = Index;
        SelectedRow = -1;
        CancelEdit();
    }
    public void SelectRow(int row) { if (row >= 0 && row < Rows.Count) SelectedRow = row; }
    private void EditRow(int row) { SelectRow(row); BeginEdit(); }
    private void RowKey(KeyboardEventArgs args, int row)
    {
        if (args.Key is "Enter" or " ") SelectRow(row);
        if (args.Key == "Enter") BeginEdit();
    }
    public void BeginAdd()
    {
        if (!CanEditNow) return;
        _editRow = -1; Editing = true; Error = null; _removing = false;
        Reference = ChargeReference = string.Empty;
        Configuration = 0; TokenProportionText = "0";
    }
    public void BeginEdit()
    {
        if (!CanEditNow || !HasSelection) return;
        _editRow = SelectedRow;
        var cells = Cells(SelectedRow).ToArray();
        Reference = cells[0];
        if (Index == 11) { ChargeReference = cells[1]; Configuration = Enum.Parse<CreditCollectionConfiguration>(cells[2]); }
        if (Index == 12) TokenProportionText = cells[1];
        Editing = true; Error = null; _removing = false;
    }
    public void BeginRemove() { BeginEdit(); if (Editing) _removing = true; }
    public void CancelEdit() { Editing = false; _editRow = -1; Error = null; _removing = false; }
    public void SaveRow()
    {
        if (!CanEditNow || !Editing) return;
        if (_removing) { RemoveSelected(); return; }
        try
        {
            var reference = LogicalNameBytes(Reference);
            object value = Index switch
            {
                9 or 10 => reference,
                11 => new GXStructure { reference, LogicalNameBytes(ChargeReference), new GXBitString((uint)Configuration, 3) },
                _ => new GXStructure { reference, byte.Parse(TokenProportionText, CultureInfo.InvariantCulture) }
            };
            var rows = Rows;
            if (_editRow < 0) rows.Add(value);
            else if (_editRow < rows.Count) rows[_editRow] = value;
            else throw new InvalidOperationException("The selected row no longer exists.");
            if (!Save(rows)) return;
            SelectedRow = _editRow < 0 ? rows.Count - 1 : _editRow;
            CancelEdit();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
    }
    public void RemoveSelected()
    {
        if (!CanEditNow || !HasSelection) return;
        var rows = Rows;
        rows.RemoveAt(SelectedRow);
        if (Save(rows)) { SelectedRow = -1; CancelEdit(); }
    }
    private bool Save(GXArray rows)
    {
        if (!GXWireValue.TryAttribute(Edit, Index, GXDLMSObjectEditState.Format(rows), out var error)) { Error = error; return false; }
        Edit.EditAttribute(Index, GXDLMSObjectEditState.Format(rows));
        Error = Edit.Error(Index);
        return Error is null;
    }
    public void Dispose() { _disposed = true; CancelEdit(); }
}
