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
using System.Text;
using Gurux.DLMS.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Gurux.UI.Components;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Edits active or passive season, week, and day profiles in an activity calendar.</summary>
public partial class GXCalendarTable
{
    /// <summary>Gets or sets the shared edit state used to read and stage attribute values.</summary>
    [Parameter, EditorRequired] public GXDLMSObjectEditState Edit { get; set; } = default!;
    /// <summary>Gets or sets the COSEM attribute index identifying the calendar profile table.</summary>
    [Parameter]
    public int Index
    {
        get; set;
    }
    /// <summary>Gets or sets the label displayed beside the field.</summary>
    [Parameter] public string Label { get; set; } = string.Empty;
    /// <summary>Gets or sets whether editing is enabled, subject to object access rights.</summary>
    [Parameter]
    public bool Enabled
    {
        get; set;
    }
    /// <summary>Gets or sets an optional callback that determines current write access.</summary>
    [Parameter]
    public Func<bool>? CanEdit
    {
        get; set;
    }
    private bool Writable => Enabled && (CanEdit?.Invoke() ?? GXDLMSObjectAccess.CanEdit(Edit.Target, Index, null));
    /// <summary>Gets the current validation or editing error, or null when no error is present.</summary>
    public string? Error
    {
        get; private set;
    }
    /// <summary>Gets the zero-based selected profile index, or -1 when no profile is selected.</summary>
    public int SelectedRow { get; private set; } = -1;
    /// <summary>Gets the zero-based selected day action index, or -1 when no action is selected.</summary>
    public int SelectedAction { get; private set; } = -1;
    /// <summary>Gets or sets the ASCII season or week profile name being edited.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Gets or sets the season start date and time in meter format.</summary>
    public string Start { get; set; } = string.Empty;
    /// <summary>Gets or sets the week profile name referenced by the season being edited.</summary>
    public string WeekName { get; set; } = string.Empty;
    /// <summary>Gets or sets the seven day profile identifiers for Monday through Sunday.</summary>
    public string[] DayIds { get; set; } = Enumerable.Repeat(string.Empty, 7).ToArray();
    /// <summary>Gets or sets the day profile identifier being edited.</summary>
    public string DayId { get; set; } = string.Empty;
    /// <summary>Gets or sets the day action start time in meter format.</summary>
    public string ActionTime { get; set; } = string.Empty;
    /// <summary>Gets or sets the six-part logical name of the day action script table.</summary>
    public string ActionScript { get; set; } = string.Empty;
    /// <summary>Gets or sets the script selector for the day action being edited.</summary>
    public string ActionSelector { get; set; } = string.Empty;
    private GXDLMSObjectEditState? _lastEdit;
    private int _lastIndex;
    private GXDLMSActivityCalendar? _snapshot;
    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _snapshot = null;
        if (ReferenceEquals(_lastEdit, Edit) && _lastIndex == Index)
        {
            return;
        }

        CancelDialog();
        _lastEdit = Edit;
        _lastIndex = Index;
        SelectedRow = SelectedAction = -1;
        Name = Start = WeekName = DayId = ActionTime = ActionScript = ActionSelector = string.Empty;
        DayIds = Enumerable.Repeat(string.Empty, 7).ToArray();
        Error = null;
    }
    private int Kind => (Index - 3) % 4;
    private bool Active => Index < 7;
    private static readonly string[] DayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    private GXDLMSActivityCalendar Calendar()
    {
        if (_snapshot is not null)
        {
            return _snapshot;
        }

        var value = (GXDLMSActivityCalendar)Edit.Target.Clone();
        if (Edit.Target.GetDirty(Index, out var pending))
        {
            var args = new ValueEventArgs(value, Index, 0, null) { Value = pending, User = true };
            ((IGXDLMSBase)value).SetValue(new GXDLMSSettings(), args);
        }
        return _snapshot = value;
    }
    private GXDLMSSeasonProfile[] Seasons(GXDLMSActivityCalendar c) => (Active ? c.SeasonProfileActive : c.SeasonProfilePassive) ?? [];
    private GXDLMSWeekProfile[] Weeks(GXDLMSActivityCalendar c) => (Active ? c.WeekProfileTableActive : c.WeekProfileTablePassive) ?? [];
    private GXDLMSDayProfile[] Days(GXDLMSActivityCalendar c) => (Active ? c.DayProfileTableActive : c.DayProfileTablePassive) ?? [];
    private int RowCount
    {
        get
        {
            var c = Calendar();
            return Kind == 0 ? Seasons(c).Length : Kind == 1 ? Weeks(c).Length : Days(c).Length;
        }
    }
    private GXDLMSDayProfileAction[] DayActions => Days(Calendar()).ElementAtOrDefault(SelectedRow)?.DaySchedules ?? [];
    private static int[] WeekDays(GXDLMSWeekProfile w) => [w.Monday, w.Tuesday, w.Wednesday, w.Thursday, w.Friday, w.Saturday, w.Sunday];
    private IEnumerable<string> RowCells(int row)
    {
        var c = Calendar();
        if (Kind == 0)
        {
            var s = Seasons(c)[row];
            return [Encoding.ASCII.GetString(s.Name ?? []), s.Start?.ToFormatMeterString(CultureInfo.InvariantCulture) ?? "", Encoding.ASCII.GetString(s.WeekName ?? [])];
        }
        if (Kind == 1)
        {
            var w = Weeks(c)[row];
            return new[] { Encoding.ASCII.GetString(w.Name ?? []) }.Concat(WeekDays(w).Select(v => v.ToString(CultureInfo.InvariantCulture)));
        }
        var d = Days(c)[row];
        return [d.DayId.ToString(CultureInfo.InvariantCulture)];
    }
    private void Change(Action<GXDLMSActivityCalendar> update)
    {
        if (!Writable)
        {
            return;
        }

        try
        {
            var c = (GXDLMSActivityCalendar)Calendar().Clone();
            update(c);
            var text = GXDLMSObjectEditState.Format(GXDLMSAttributeCodec.Read(c, Index));
            var isolated = GXDLMSObjectEditState.For((GXDLMSActivityCalendar)Edit.Target.Clone());
            isolated.EditAttribute(Index, text);
            if (!isolated.IsValid)
            {
                throw new FormatException(isolated.Error(Index));
            }

            Edit.EditAttribute(Index, text);
            Error = Edit.Error(Index);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException) { Error = ex.Message; }
        finally { _snapshot = null; }
    }
    private void SetRows<T>(GXDLMSActivityCalendar c, T[] rows)
    {
        if (rows is GXDLMSSeasonProfile[] seasons)
        {
            if (Active)
            {
                c.SeasonProfileActive = seasons;
            }
            else
            {
                c.SeasonProfilePassive = seasons;
            }
        }
        else if (rows is GXDLMSWeekProfile[] weeks)
        {
            if (Active)
            {
                c.WeekProfileTableActive = weeks;
            }
            else
            {
                c.WeekProfileTablePassive = weeks;
            }
        }
        else if (rows is GXDLMSDayProfile[] days)
        {
            if (Active)
            {
                c.DayProfileTableActive = days;
            }
            else
            {
                c.DayProfileTablePassive = days;
            }
        }
    }
    private static byte[] NameBytes(string value) => !string.IsNullOrEmpty(value) && value.All(c => c <= 127) ? Encoding.ASCII.GetBytes(value) : throw new FormatException("Enter an ASCII profile name.");
    /// <summary>Loads a profile into the edit dialog.</summary>
    /// <param name="row">The zero-based profile index.</param>
    public void LoadRow(int row)
    {
        if (!Writable)
        {
            return;
        }

        OpenDialog(false, false);
        var c = Calendar();
        SelectedRow = row;
        SelectedAction = -1;
        if (Kind == 0)
        {
            var s = Seasons(c)[row];
            Name = Encoding.ASCII.GetString(s.Name ?? []);
            Start = s.Start?.ToFormatMeterString(CultureInfo.InvariantCulture) ?? string.Empty;
            WeekName = Encoding.ASCII.GetString(s.WeekName ?? []);
        }
        else if (Kind == 1)
        {
            var w = Weeks(c)[row];
            Name = Encoding.ASCII.GetString(w.Name ?? []);
            DayIds = WeekDays(w).Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray();
        }
        else
        {
            DayId = Days(c)[row].DayId.ToString(CultureInfo.InvariantCulture);
        }
    }
    /// <summary>Validates the current profile fields and stages a new profile row.</summary>
    public void AddRow() => StoreRow(-1);
    /// <summary>Validates the current profile fields and stages changes to the selected profile.</summary>
    public void SaveRow()
    {
        if (SelectedRow >= 0)
        {
            StoreRow(SelectedRow);
        }
    }
    private void StoreRow(int row) => Change(c =>
    {
        if (Kind == 0)
        {
            var rows = Seasons(c).ToList();
            var s = new GXDLMSSeasonProfile { Name = NameBytes(Name), Start = new GXDateTime(Start, CultureInfo.InvariantCulture), WeekName = NameBytes(WeekName) };
            if (row < 0)
            {
                rows.Add(s);
            }
            else
            {
                rows[row] = s;
            }

            SetRows(c, rows.ToArray());
            SelectedRow = row < 0 ? rows.Count - 1 : row;
        }
        else if (Kind == 1)
        {
            var d = DayIds.Select(v => byte.Parse(v, CultureInfo.InvariantCulture)).ToArray();
            if (d.Length != 7)
            {
                throw new FormatException("Enter seven day IDs.");
            }

            var w = new GXDLMSWeekProfile { Name = NameBytes(Name), Monday = d[0], Tuesday = d[1], Wednesday = d[2], Thursday = d[3], Friday = d[4], Saturday = d[5], Sunday = d[6] };
            var rows = Weeks(c).ToList();
            if (row < 0)
            {
                rows.Add(w);
            }
            else
            {
                rows[row] = w;
            }

            SetRows(c, rows.ToArray());
            SelectedRow = row < 0 ? rows.Count - 1 : row;
        }
        else
        {
            var rows = Days(c).ToList();
            var id = byte.Parse(DayId, CultureInfo.InvariantCulture);
            if (rows.Where((_, i) => i != row).Any(d => d.DayId == id))
            {
                throw new FormatException("Day IDs must be unique.");
            }

            var d = new GXDLMSDayProfile { DayId = id, DaySchedules = row < 0 ? [] : rows[row].DaySchedules };
            if (row < 0)
            {
                rows.Add(d);
            }
            else
            {
                rows[row] = d;
            }

            SetRows(c, rows.ToArray());
            SelectedRow = row < 0 ? rows.Count - 1 : row;
        }
    });
    /// <summary>Stages removal of a profile and clears the current selection.</summary>
    /// <param name="row">The zero-based profile index to remove.</param>
    public void RemoveRow(int row) => Change(c => { if (Kind == 0) { SetRows(c, Seasons(c).Where((_, i) => i != row).ToArray()); } else if (Kind == 1) { SetRows(c, Weeks(c).Where((_, i) => i != row).ToArray()); } else { SetRows(c, Days(c).Where((_, i) => i != row).ToArray()); } SelectedRow = -1; SelectedAction = -1; });
    /// <summary>Loads an action from the selected day profile into the edit dialog.</summary>
    /// <param name="row">The zero-based action index within the selected day.</param>
    public void LoadAction(int row)
    {
        if (!Writable)
        {
            return;
        }
        OpenDialog(true, false);
        var a = DayActions[row];
        SelectedAction = row;
        ActionTime = a.StartTime?.ToFormatMeterString(CultureInfo.InvariantCulture) ?? string.Empty;
        ActionScript = a.ScriptLogicalName ?? string.Empty;
        ActionSelector = a.ScriptSelector.ToString(CultureInfo.InvariantCulture);
    }
    /// <summary>Validates the current action fields and stages a new action in the selected day.</summary>
    public void AddAction() => StoreAction(-1);
    /// <summary>Validates the current action fields and stages changes to the selected action.</summary>
    public void SaveAction()
    {
        if (SelectedAction >= 0)
        {
            StoreAction(SelectedAction);
        }
    }
    private void StoreAction(int row) => Change(c =>
    {
        if (Kind != 2 || SelectedRow < 0)
        {
            throw new FormatException("Select a day first.");
        }

        var parts = ActionScript.Split('.');
        if (parts.Length != 6 || parts.Any(p => !byte.TryParse(p, out _)))
        {
            throw new FormatException("A script logical name requires six byte values.");
        }

        var action = new GXDLMSDayProfileAction { StartTime = new GXTime(ActionTime, CultureInfo.InvariantCulture), ScriptLogicalName = ActionScript, ScriptSelector = ushort.Parse(ActionSelector, CultureInfo.InvariantCulture) };
        var day = Days(c)[SelectedRow];
        var rows = (day.DaySchedules ?? []).ToList();
        if (row < 0)
        {
            rows.Add(action);
        }
        else
        {
            rows[row] = action;
        }

        day.DaySchedules = rows.ToArray();
    });
    /// <summary>Stages removal of an action from the selected day profile.</summary>
    /// <param name="row">The zero-based action index to remove.</param>
    public void RemoveAction(int row) => Change(c => { var day = Days(c)[SelectedRow]; day.DaySchedules = (day.DaySchedules ?? []).Where((_, i) => i != row).ToArray(); SelectedAction = -1; });
    /// <summary>Gets whether an editing session is open.</summary>
    public bool Editing
    {
        get; private set;
    }
    private bool _editingAction;
    private bool _removing;
    private bool _adding;
    private GXDLMSObjectEditState? _dialogEdit;
    private int _dialogIndex;
    private string? _sourceText;
    private string[]? _savedFields;
    private int _savedRow, _savedAction;
    private string TableKey => $"{Index}:{Edit.AttributeText(Index)}";
    private string ActionTableKey => $"{TableKey}:{SelectedRow}";
    private string[] Headers => Kind == 0 ? ["Name", "Start", "Week Name"] : Kind == 1 ? ["Name", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"] : ["Day ID"];
    private string DialogTitle => _removing ? "Remove" : _editingAction ? "Day action" : Label;
    private ValueTask<ItemsProviderResult<int>> GetRows(GXItemsProviderRequest request) => ValueTask.FromResult(new ItemsProviderResult<int>(Enumerable.Range(0, RowCount), RowCount));
    private ValueTask<ItemsProviderResult<int>> GetActions(GXItemsProviderRequest request) => ValueTask.FromResult(new ItemsProviderResult<int>(Enumerable.Range(0, DayActions.Length), DayActions.Length));
    /// <summary>Selects a day profile and clears the action selection.</summary>
    /// <param name="row">The zero-based day profile index.</param>
    public void SelectDay(int row)
    {
        SelectedRow = row;
        SelectedAction = -1;
    }
    private void OpenDialog(bool action, bool removing)
    {
        _savedFields = [Name, Start, WeekName, DayId, ActionTime, ActionScript, ActionSelector, .. DayIds];
        _savedRow = SelectedRow;
        _savedAction = SelectedAction;
        _dialogEdit = Edit;
        _dialogIndex = Index;
        _sourceText = Edit.AttributeText(Index);
        _editingAction = action;
        _removing = removing;
        _adding = false;
        Editing = true;
        Error = null;
    }
    /// <summary>Opens an empty profile dialog for adding a row.</summary>
    public void BeginAddRow()
    {
        if (!Writable)
        {
            return;
        }

        OpenDialog(false, false);
        _adding = true;
        Name = Start = WeekName = DayId = string.Empty;
        DayIds = Enumerable.Repeat(string.Empty, 7).ToArray();
    }
    /// <summary>Opens an empty action dialog for the selected day profile.</summary>
    public void BeginAddAction()
    {
        if (!Writable || SelectedRow < 0)
        {
            return;
        }

        OpenDialog(true, false);
        _adding = true;
        ActionTime = ActionScript = ActionSelector = string.Empty;
    }
    /// <summary>Opens a confirmation dialog for removing a profile or day action.</summary>
    /// <param name="row">The zero-based profile or action index to remove.</param>
    /// <param name="action">Whether the row identifies an action in the selected day profile.</param>
    public void BeginRemove(int row, bool action)
    {
        if (!Writable)
        {
            return;
        }

        OpenDialog(action, true);
        if (action)
        {
            SelectedAction = row;
        }
        else
        {
            SelectedRow = row;
        }
    }
    /// <summary>Validates and stages the dialog change if its target, access, and source value are unchanged.</summary>
    public void AcceptDialog()
    {
        if (!Editing)
        {
            return;
        }

        if (!Writable || !ReferenceEquals(Edit, _dialogEdit) || Index != _dialogIndex || Edit.AttributeText(Index) != _sourceText)
        {
            Error = "The target, access rights or pending value changed. Cancel and open the editor again.";
            return;
        }
        Error = null;
        if (_removing)
        {
            if (_editingAction)
            {
                RemoveAction(SelectedAction);
            }
            else
            {
                RemoveRow(SelectedRow);
            }
        }
        else if (_editingAction)
        {
            if (_adding)
            {
                AddAction();
            }
            else
            {
                SaveAction();
            }
        }
        else
        {
            if (_adding)
            {
                AddRow();
            }
            else
            {
                SaveRow();
            }
        }
        if (Error is null)
        {
            Editing = false;
            _savedFields = null;
            _dialogEdit = null;
        }
    }
    /// <summary>Closes the dialog and restores the fields and selection captured when it opened.</summary>
    public void CancelDialog()
    {
        if (_savedFields is { Length: 14 } fields)
        {
            Name = fields[0];
            Start = fields[1];
            WeekName = fields[2];
            DayId = fields[3];
            ActionTime = fields[4];
            ActionScript = fields[5];
            ActionSelector = fields[6];
            DayIds = fields.Skip(7).ToArray();
            SelectedRow = _savedRow;
            SelectedAction = _savedAction;
        }
        Editing = false;
        _savedFields = null;
        _dialogEdit = null;
        Error = null;
    }
}
