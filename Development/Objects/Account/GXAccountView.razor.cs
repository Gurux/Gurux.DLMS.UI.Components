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
using Gurux.DLMS.Objects;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Displays account settings and stages edits and account method invocations.</summary>
public partial class GXAccountView
{
    /// <summary>Provides a unique identifier prefix for account credit-status switches.</summary>
    private readonly string _creditStatusId = $"account-credit-status-{Guid.NewGuid():N}";
    /// <summary>Maps the account credit attributes to their displayed captions.</summary>
    private static (int Index, string Label)[] CreditFields => [(5, Properties.Resources.AvailableCreditLabel), (6, Properties.Resources.AmountToClearLabel), (7, Properties.Resources.ClearanceThresholdLabel), (8, Properties.Resources.AggregatedDebtLabel)];
    /// <summary>Maps the account provision attributes to their displayed captions.</summary>
    private static (int Index, string Label)[] ProvisionFields => [(16, Properties.Resources.LowCreditThresholdLabel), (17, Properties.Resources.NextCreditAvailableThresholdLabel), (18, Properties.Resources.MaxProvisionLabel), (19, Properties.Resources.MaxProvisionPeriodLabel)];

    /// <summary>Copies the current account structure, preserving other fields from an invalid draft.</summary>
    /// <param name="index">The one-based COSEM attribute or method index.</param>
    /// <returns>Copies the current account structure, preserving other fields from an invalid draft.</returns>
    private GXStructure Structure(int index)
    {
        // Invalid compound drafts still contain the other edited fields. Keep
        // those fields when the user corrects one part of the structure.
        var source = Edit.Error(index) is not null
            ? GXDLMSTranslator.XmlToValue(Edit.AttributeText(index))
            : Target.GetDirty(index, out var pending) && pending is GXStructure
                ? pending : GXDLMSAttributeCodec.Read(Target, index);
        var result = new GXStructure();
        result.AddRange((GXStructure)source!);
        return result;
    }
    /// <summary>Stages one member of an account structure when the attribute is writable.</summary>
    /// <param name="index">The one-based COSEM attribute or method index.</param>
    /// <param name="part">The zero-based member index within the attribute structure.</param>
    /// <param name="value">The input value to convert, format or stage.</param>
    private void ChangePart(int index, int part, object value)
    {
        if (!CanEditAttribute(index))
        {
            return;
        }

        var structure = Structure(index);
        structure[part] = value;
        Edit.EditAttribute(index, GXDLMSObjectEditState.Format(structure));
    }
    /// <summary>Gets or sets the payment mode in the account mode and status draft.</summary>
    public PaymentMode PaymentModeValue
    {
        get => (PaymentMode)Convert.ToByte(Structure(2)[0], CultureInfo.InvariantCulture);
        set
        {
            if (Enum.IsDefined(value))
            {
                ChangePart(2, 0, new GXEnum((byte)value));
            }
        }
    }
    /// <summary>Gets or sets the account status in the account mode and status draft.</summary>
    public AccountStatus AccountStatusValue
    {
        get => (AccountStatus)Convert.ToByte(Structure(2)[1], CultureInfo.InvariantCulture);
        set
        {
            if (Enum.IsDefined(value))
            {
                ChangePart(2, 1, new GXEnum((byte)value));
            }
        }
    }
    /// <summary>Gets or sets the currency name in the currency draft.</summary>
    public string CurrencyName
    {
        get => Convert.ToString(Structure(15)[0], CultureInfo.InvariantCulture) ?? string.Empty;
        set => ChangePart(15, 0, value);
    }
    /// <summary>Gets or sets the currency scale text, retaining invalid input for validation.</summary>
    public string CurrencyScaleText
    {
        get => Convert.ToString(Structure(15)[1], CultureInfo.InvariantCulture) ?? "0";
        set
        {
            if (!CanEditAttribute(15))
            {
                return;
            }
            // Keep invalid drafts in the shared edit state so writes remain blocked until corrected.
            ChangePart(15, 1, sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var scale) ? (object)scale : value);
        }
    }
    /// <summary>Gets or sets the currency unit in the currency draft.</summary>
    public Currency CurrencyUnitValue
    {
        get => (Currency)Convert.ToByte(Structure(15)[2], CultureInfo.InvariantCulture);
        set
        {
            if (Enum.IsDefined(value))
            {
                ChangePart(15, 2, new GXEnum((byte)value));
            }
        }
    }
    /// <summary>Gets the account credit status flags from the current attribute value.</summary>
    public AccountCreditStatus CreditStatus => Edit.AttributeValue(4) is GXBitString bits
        ? (AccountCreditStatus)bits.ToInteger() : (AccountCreditStatus)Convert.ToByte(Edit.AttributeValue(4), CultureInfo.InvariantCulture);
    /// <summary>Stages a change to the account credit status flags.</summary>
    /// <param name="flag">The flag to change, or zero to clear all flags.</param>
    /// <param name="enabled">Whether to set the specified flag.</param>
    public void ChangeCreditStatus(AccountCreditStatus flag, bool enabled)
    {
        if (!CanEditAttribute(4))
        {
            return;
        }

        var value = flag == 0 ? 0 : enabled ? CreditStatus | flag : CreditStatus & ~flag;
        Edit.EditAttribute(4, new GXBitString((uint)value, 8).ToString());
    }
    /// <summary>Stages text for an editable scalar account attribute.</summary>
    /// <param name="index">The COSEM attribute index.</param>
    /// <param name="text">The attribute value in its editable text representation.</param>
    public void ChangeAttribute(int index, string text)
    {
        if (index is < 3 or > 19 or 4 or >= 9 and <= 12 or 15 || !CanEditAttribute(index))
        {
            return;
        }

        Edit.EditAttribute(index, text);
    }
    /// <summary>Invokes the account activation method.</summary>
    /// <returns>A task that completes when the method invocation finishes.</returns>
    public System.Threading.Tasks.Task ActivateAsync() => InvokeMethodAsync(1, client => Target.Activate(client));
    /// <summary>Invokes the account closing method.</summary>
    /// <returns>A task that completes when the method invocation finishes.</returns>
    public System.Threading.Tasks.Task CloseAsync() => InvokeMethodAsync(2, client => Target.Close(client));
    /// <summary>Invokes the account reset method.</summary>
    /// <returns>A task that completes when the method invocation finishes.</returns>
    public System.Threading.Tasks.Task ResetAsync() => InvokeMethodAsync(3, client => Target.Reset(client));
}
