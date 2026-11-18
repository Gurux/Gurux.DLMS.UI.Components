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
    /// <summary>Gets or sets the shared edit state used to read and stage attribute values.</summary>
    [Parameter, EditorRequired] public GXDLMSObjectEditState Edit { get; set; } = default!;
    /// <summary>Gets or sets the account attribute index, from 9 through 12, identifying the reference table.</summary>
    [Parameter]
    public int Index
    {
        get; set;
    }
    /// <summary>Gets or sets whether editing is enabled, subject to object access rights.</summary>
    [Parameter]
    public bool Enabled
    {
        get; set;
    }
    /// <summary>Gets or sets the cascading access rules for the target object.</summary>
    [CascadingParameter]
    public GXObjectEditorAccess? Access
    {
        get; set;
    }
    /// <summary>Prevents callbacks from updating a component after disposal.</summary>
    private bool _disposed;
    /// <summary>Gets whether the current target and access rights permit editing.</summary>
    private bool CanEditNow => !_disposed && Enabled && (Access is null ? GXDLMSObjectAccess.CanEdit(Edit.Target, Index) : ReferenceEquals(Access.Target, Edit.Target) && Access.CanEdit(Index));
    /// <summary>Gets the zero-based selected row index, or -1 when no row is selected.</summary>
    public int SelectedRow { get; private set; } = -1;
    /// <summary>Gets whether an editing session is open.</summary>
    public bool Editing
    {
        get; private set;
    }
    /// <summary>Gets the current validation or editing error, or null when no error is present.</summary>
    public string? Error
    {
        get; private set;
    }
    /// <summary>Gets or sets the credit or charge logical name being edited.</summary>
    public string Reference { get; set; } = string.Empty;
    /// <summary>Gets or sets the charge logical name in a credit-charge configuration row.</summary>
    public string ChargeReference { get; set; } = string.Empty;
    /// <summary>Gets or sets the credit collection flags for a credit-charge configuration row.</summary>
    public CreditCollectionConfiguration Configuration
    {
        get; set;
    }
    /// <summary>Gets or sets the token proportion text for a token gateway configuration row.</summary>
    public string TokenProportionText { get; set; } = "0";
    /// <summary>Stores the row being edited, with a negative value indicating a new row.</summary>
    private int _editRow = -1;
    /// <summary>Tracks whether the current dialog confirms row removal.</summary>
    private bool _removing;
    /// <summary>Supplies the current row indices and total count to the table.</summary>
    /// <param name="request">The table request; this provider returns the complete current row set.</param>
    /// <returns>The current table rows together with their total count.</returns>
    private ValueTask<Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<int>> ProvideRows(Gurux.UI.Components.GXItemsProviderRequest request)
        => ValueTask.FromResult(new Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult<int>(Enumerable.Range(0, Rows.Count), Rows.Count));
    /// <summary>Retains the previously rendered edit state to detect target changes.</summary>
    private GXDLMSObjectEditState? _lastEdit;
    /// <summary>Retains the previously rendered attribute index.</summary>
    private int _lastIndex;
    /// <summary>Identifies the datalist containing compatible object references.</summary>
    private readonly string _referenceListId = $"account-references-{Guid.NewGuid():N}";
    /// <summary>Identifies the datalist containing charge object references.</summary>
    private readonly string _chargeListId = $"account-charges-{Guid.NewGuid():N}";
    /// <summary>Gets the title for the current account table attribute.</summary>
    private string Title => Index switch { 9 => Properties.Resources.CreditReference2, 10 => Properties.Resources.ChargeReference, 11 => Properties.Resources.CreditCharge, _ => Properties.Resources.TokenGateway };
    /// <summary>Gets the column captions for the current table variant.</summary>
    private string[] Headers => Index switch { 9 or 10 => [Properties.Resources.Name], 11 => [Properties.Resources.Credit, Properties.Resources.Charge, Properties.Resources.Configuration], _ => [Properties.Resources.CreditReference, Properties.Resources.TokenProportion] };
    /// <summary>Gets the table entries from the pending or confirmed attribute value.</summary>
    private GXArray Rows
    {
        get
        {
            var rows = new GXArray();
            rows.AddRange((GXArray)(Edit.Target.GetDirty(Index, out var pending) ? pending : GXDLMSAttributeCodec.Read(Edit.Target, Index))!);
            return rows;
        }
    }
    /// <summary>Gets whether the selected row still exists in the current table.</summary>
    private bool HasSelection => SelectedRow >= 0 && SelectedRow < Rows.Count;
    /// <summary>Gets the logical names of compatible credit or charge objects.</summary>
    private IEnumerable<string> ReferenceOptions => ObjectNames(Index == 10 ? ObjectType.Charge : ObjectType.Credit);
    /// <summary>Gets the logical names of charge objects in the same object collection.</summary>
    private IEnumerable<string> ChargeOptions => ObjectNames(ObjectType.Charge);
    /// <summary>Enumerates logical names for objects of the requested COSEM class.</summary>
    /// <param name="type">The COSEM data type or object class being validated.</param>
    /// <returns>Enumerates logical names for objects of the requested COSEM class.</returns>
    private IEnumerable<string> ObjectNames(ObjectType type) => Edit.Target.Parent?.GetObjects(type).Select(item => item.LogicalName) ?? [];
    /// <summary>Formats a byte buffer or scalar as a logical-name string.</summary>
    /// <param name="value">The input value to convert, format or stage.</param>
    /// <returns>The formatted text for the supplied value.</returns>
    private static string LogicalName(object value) => value is byte[] bytes ? string.Join(".", bytes) : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    /// <summary>Validates and encodes the six components of a logical name.</summary>
    /// <param name="text">The user input in the editor&apos;s invariant text representation.</param>
    /// <returns>Validates and encodes the six components of a logical name.</returns>
    private static byte[] LogicalNameBytes(string text)
    {
        var parts = text.Split('.');
        if (parts.Length != 6 || parts.Any(part => !byte.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out _)))
        {
            throw new FormatException(Properties.Resources.EnterALogicalNameWithSixNumbersBetween0And255);
        }

        return parts.Select(part => byte.Parse(part, CultureInfo.InvariantCulture)).ToArray();
    }
    /// <summary>Formats the selected row into the table&apos;s visible cells.</summary>
    /// <param name="row">The zero-based row index in the current table.</param>
    /// <returns>The formatted text for the supplied value.</returns>
    private IEnumerable<string> Cells(int row)
    {
        var value = Rows[row];
        if (Index <= 10)
        {
            return [LogicalName(value)];
        }

        var cells = (GXStructure)value;
        return Index == 11
            ? [LogicalName(cells[0]), LogicalName(cells[1]), ((CreditCollectionConfiguration)Convert.ToByte(cells[2], CultureInfo.InvariantCulture)).ToString()]
            : [LogicalName(cells[0]), Convert.ToString(cells[1], CultureInfo.InvariantCulture) ?? "0"];
    }
    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Index is < 9 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(Index));
        }

        if (ReferenceEquals(_lastEdit, Edit) && _lastIndex == Index)
        {
            return;
        }

        _lastEdit = Edit;
        _lastIndex = Index;
        SelectedRow = -1;
        CancelEdit();
    }
    /// <summary>Selects an existing account reference row.</summary>
    /// <param name="row">The zero-based row index to select.</param>
    public void SelectRow(int row)
    {
        if (row >= 0 && row < Rows.Count)
        {
            SelectedRow = row;
        }
    }
    /// <summary>Selects a row and opens its edit dialog when editing is permitted.</summary>
    /// <param name="row">The zero-based row index in the current table.</param>
    private void EditRow(int row)
    {
        SelectRow(row);
        BeginEdit();
    }
    /// <summary>Opens the selected row for editing when Enter or Space is pressed.</summary>
    /// <param name="args">The browser input event received by the component.</param>
    /// <param name="row">The zero-based row index in the current table.</param>
    private void RowKey(KeyboardEventArgs args, int row)
    {
        if (args.Key is "Enter" or " ")
        {
            SelectRow(row);
        }

        if (args.Key == "Enter")
        {
            BeginEdit();
        }
    }
    /// <summary>Opens an empty editing session for a new reference row.</summary>
    public void BeginAdd()
    {
        if (!CanEditNow)
        {
            return;
        }

        _editRow = -1;
        Editing = true;
        Error = null;
        _removing = false;
        Reference = ChargeReference = string.Empty;
        Configuration = 0;
        TokenProportionText = "0";
    }
    /// <summary>Loads the selected reference row into an editing session.</summary>
    public void BeginEdit()
    {
        if (!CanEditNow || !HasSelection)
        {
            return;
        }

        _editRow = SelectedRow;
        var cells = Cells(SelectedRow).ToArray();
        Reference = cells[0];
        if (Index == 11)
        {
            ChargeReference = cells[1];
            Configuration = Enum.Parse<CreditCollectionConfiguration>(cells[2]);
        }
        if (Index == 12)
        {
            TokenProportionText = cells[1];
        }

        Editing = true;
        Error = null;
        _removing = false;
    }
    /// <summary>Opens a removal confirmation session for the selected reference row.</summary>
    public void BeginRemove()
    {
        BeginEdit();
        if (Editing)
        {
            _removing = true;
        }
    }
    /// <summary>Discards the current editing session and clears its error.</summary>
    public void CancelEdit()
    {
        Editing = false;
        _editRow = -1;
        Error = null;
        _removing = false;
    }
    /// <summary>Validates and stages the edited reference row or the confirmed removal.</summary>
    public void SaveRow()
    {
        if (!CanEditNow || !Editing)
        {
            return;
        }

        if (_removing)
        {
            RemoveSelected();
            return;
        }
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
            if (_editRow < 0)
            {
                rows.Add(value);
            }
            else if (_editRow < rows.Count)
            {
                rows[_editRow] = value;
            }
            else
            {
                throw new InvalidOperationException(Properties.Resources.TheSelectedRowNoLongerExists);
            }

            if (!Save(rows))
            {
                return;
            }

            SelectedRow = _editRow < 0 ? rows.Count - 1 : _editRow;
            CancelEdit();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
    }
    /// <summary>Stages removal of the selected reference row and clears the selection on success.</summary>
    public void RemoveSelected()
    {
        if (!CanEditNow || !HasSelection)
        {
            return;
        }

        var rows = Rows;
        rows.RemoveAt(SelectedRow);
        if (Save(rows))
        {
            SelectedRow = -1;
            CancelEdit();
        }
    }
    /// <summary>Stages the edited account table and reports whether validation succeeded.</summary>
    /// <param name="rows">The replacement profile or reference rows to stage.</param>
    /// <returns>True when the requested condition is satisfied; otherwise, false.</returns>
    private bool Save(GXArray rows)
    {
        if (!GXWireValue.TryAttribute(Edit, Index, GXDLMSObjectEditState.Format(rows), out var error))
        {
            Error = error;
            return false;
        }
        Edit.EditAttribute(Index, GXDLMSObjectEditState.Format(rows));
        Error = Edit.Error(Index);
        return Error is null;
    }
    /// <summary>Prevents further editing and discards the current editing session.</summary>
    public void Dispose()
    {
        _disposed = true;
        CancelEdit();
    }
}
