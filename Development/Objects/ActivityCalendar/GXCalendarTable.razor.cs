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
    /// <summary>Gets whether the current access checks permit calendar editing.</summary>
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
    /// <summary>Retains the previously rendered edit state to detect target changes.</summary>
    private GXDLMSObjectEditState? _lastEdit;
    /// <summary>Retains the previously rendered attribute index.</summary>
    private int _lastIndex;
    /// <summary>Caches the cloned calendar used during the current edit operation.</summary>
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
    /// <summary>Gets the input or table variant selected for the current value.</summary>
    private int Kind => (Index - 3) % 4;
    /// <summary>Gets whether the selected attribute belongs to the active calendar.</summary>
    private bool Active => Index < 7;
    /// <summary>Lists the weekday captions in Monday-to-Sunday order.</summary>
    private static string[] DayNames => [Properties.Resources.Monday, Properties.Resources.Tuesday, Properties.Resources.Wednesday, Properties.Resources.Thursday, Properties.Resources.Friday, Properties.Resources.Saturday, Properties.Resources.Sunday];
    /// <summary>Gets a cloned calendar with the selected pending attribute applied.</summary>
    /// <returns>A cloned calendar with the selected pending attribute applied.</returns>
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
    /// <summary>Gets the season profiles from the selected active or passive calendar.</summary>
    /// <param name="c">The calendar clone containing the active and passive profile tables.</param>
    /// <returns>The season profiles from the selected active or passive calendar.</returns>
    private GXDLMSSeasonProfile[] Seasons(GXDLMSActivityCalendar c) => (Active ? c.SeasonProfileActive : c.SeasonProfilePassive) ?? [];
    /// <summary>Gets the week profiles from the selected active or passive calendar.</summary>
    /// <param name="c">The calendar clone containing the active and passive profile tables.</param>
    /// <returns>The week profiles from the selected active or passive calendar.</returns>
    private GXDLMSWeekProfile[] Weeks(GXDLMSActivityCalendar c) => (Active ? c.WeekProfileTableActive : c.WeekProfileTablePassive) ?? [];
    /// <summary>Gets the day profiles from the selected active or passive calendar.</summary>
    /// <param name="c">The calendar clone containing the active and passive profile tables.</param>
    /// <returns>The day profiles from the selected active or passive calendar.</returns>
    private GXDLMSDayProfile[] Days(GXDLMSActivityCalendar c) => (Active ? c.DayProfileTableActive : c.DayProfileTablePassive) ?? [];
    /// <summary>Gets the number of profiles in the selected calendar table.</summary>
    private int RowCount
    {
        get
        {
            var c = Calendar();
            return Kind == 0 ? Seasons(c).Length : Kind == 1 ? Weeks(c).Length : Days(c).Length;
        }
    }
    /// <summary>Gets the actions of the selected day profile, or an empty array.</summary>
    private GXDLMSDayProfileAction[] DayActions => Days(Calendar()).ElementAtOrDefault(SelectedRow)?.DaySchedules ?? [];
    /// <summary>Returns a week&apos;s seven day identifiers in Monday-to-Sunday order.</summary>
    /// <param name="w">The week profile containing seven day identifiers.</param>
    /// <returns>A week&apos;s seven day identifiers in Monday-to-Sunday order.</returns>
    private static int[] WeekDays(GXDLMSWeekProfile w) => [w.Monday, w.Tuesday, w.Wednesday, w.Thursday, w.Friday, w.Saturday, w.Sunday];
    /// <summary>Formats a calendar profile into the cells of its table row.</summary>
    /// <param name="row">The zero-based row index in the current table.</param>
    /// <returns>The formatted text for the supplied value.</returns>
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
    /// <summary>Validates a calendar update on a clone and stages the changed profile attribute.</summary>
    /// <param name="update">The operation applied to an isolated calendar clone.</param>
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
    /// <summary>Replaces the selected active or passive profile table on a calendar clone.</summary>
    /// <param name="c">The calendar clone containing the active and passive profile tables.</param>
    /// <param name="rows">The replacement profile or reference rows to stage.</param>
    /// <typeparam name="T">The profile element type stored in the selected calendar table.</typeparam>
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
    /// <summary>Validates and encodes an ASCII calendar profile name.</summary>
    /// <param name="value">The input value to convert, format or stage.</param>
    /// <returns>Validates and encodes an ASCII calendar profile name.</returns>
    private static byte[] NameBytes(string value) => !string.IsNullOrEmpty(value) && value.All(c => c <= 127) ? Encoding.ASCII.GetBytes(value) : throw new FormatException(Properties.Resources.EnterAnASCIIProfileName);
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
    /// <summary>Validates the dialog fields and adds or replaces a calendar profile.</summary>
    /// <param name="row">The zero-based row index in the current table.</param>
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
                throw new FormatException(Properties.Resources.EnterSevenDayIDs);
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
                throw new FormatException(Properties.Resources.DayIDsMustBeUnique);
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
    /// <summary>Validates the dialog fields and adds or replaces a day action.</summary>
    /// <param name="row">The zero-based row index in the current table.</param>
    private void StoreAction(int row) => Change(c =>
    {
        if (Kind != 2 || SelectedRow < 0)
        {
            throw new FormatException(Properties.Resources.SelectADayFirst);
        }

        var parts = ActionScript.Split('.');
        if (parts.Length != 6 || parts.Any(p => !byte.TryParse(p, out _)))
        {
            throw new FormatException(Properties.Resources.AScriptLogicalNameRequiresSixByteValues);
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
    /// <summary>Tracks whether the calendar dialog edits a day action instead of a profile.</summary>
    private bool _editingAction;
    /// <summary>Tracks whether the current dialog confirms row removal.</summary>
    private bool _removing;
    /// <summary>Tracks whether the calendar dialog adds a new profile or action.</summary>
    private bool _adding;
    /// <summary>Retains the edit state captured when the calendar dialog opened.</summary>
    private GXDLMSObjectEditState? _dialogEdit;
    /// <summary>Retains the attribute index captured when the calendar dialog opened.</summary>
    private int _dialogIndex;
    /// <summary>Retains the original attribute text used to reject stale calendar edits.</summary>
    private string? _sourceText;
    /// <summary>Snapshots the calendar dialog fields so cancellation can restore them.</summary>
    private string[]? _savedFields;
    /// <summary>Snapshots the profile selection before opening a calendar dialog. Snapshots the day-action selection before opening a calendar dialog.</summary>
    private int _savedRow, _savedAction;
    /// <summary>Gets a render key that changes when the selected attribute value changes.</summary>
    private string TableKey => $"{Index}:{Edit.AttributeText(Index)}";
    /// <summary>Gets a render key for the actions of the selected day profile.</summary>
    private string ActionTableKey => $"{TableKey}:{SelectedRow}";
    /// <summary>Gets the column captions for the current table variant.</summary>
    private string[] Headers => Kind == 0 ? [Properties.Resources.Name, Properties.Resources.Start, Properties.Resources.WeekName] : Kind == 1 ? [Properties.Resources.Name, Properties.Resources.Mon, Properties.Resources.Tue, Properties.Resources.Wed, Properties.Resources.Thu, Properties.Resources.Fri, Properties.Resources.Sat, Properties.Resources.Sun] : [Properties.Resources.DayID];
    /// <summary>Gets the caption for the current profile or action dialog operation.</summary>
    private string DialogTitle => _removing ? Properties.Resources.Remove : _editingAction ? Properties.Resources.DayAction : Label;
    /// <summary>Supplies calendar profile indices and their total count to the table.</summary>
    /// <param name="request">The table request; this provider returns the complete current row set.</param>
    /// <returns>The current table rows together with their total count.</returns>
    private ValueTask<ItemsProviderResult<int>> GetRows(GXItemsProviderRequest request) => ValueTask.FromResult(new ItemsProviderResult<int>(Enumerable.Range(0, RowCount), RowCount));
    /// <summary>Supplies the selected day&apos;s action indices and their total count to the table.</summary>
    /// <param name="request">The table request; this provider returns the complete current row set.</param>
    /// <returns>The current table rows together with their total count.</returns>
    private ValueTask<ItemsProviderResult<int>> GetActions(GXItemsProviderRequest request) => ValueTask.FromResult(new ItemsProviderResult<int>(Enumerable.Range(0, DayActions.Length), DayActions.Length));
    /// <summary>Selects a day profile and clears the action selection.</summary>
    /// <param name="row">The zero-based day profile index.</param>
    public void SelectDay(int row)
    {
        SelectedRow = row;
        SelectedAction = -1;
    }
    /// <summary>Snapshots the calendar selection and fields before opening the edit dialog.</summary>
    /// <param name="action">Whether the dialog edits a day action instead of a calendar profile.</param>
    /// <param name="removing">Whether the dialog confirms removal of the selected entry.</param>
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
            Error = Properties.Resources.TheTargetAccessRightsOrPendingValueChangedCancelAndOpenTheEditorAgain;
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
