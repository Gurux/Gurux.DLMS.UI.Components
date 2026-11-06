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

using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.UI.Components.Objects;

/// <summary>Uses the object's wire representation for collections whose CLR model is not GXArray/GXStructure.</summary>
internal static class GXDLMSAttributeCodec
{
    public static object? Read(GXDLMSObject target, int index)
    {
        if (GXDLMSObjectEditState.For(target).TryGetConfirmedAttribute(index, out var confirmed))
        {
            if (target is GXDLMSPppSetup && index == 5 && confirmed is null)
            {
                return new GXStructure { Array.Empty<byte>(), Array.Empty<byte>() };
            }

            var model = target.GetValues().ElementAtOrDefault(index - 1);
            if (model is Enum && confirmed is not null)
            {
                return Enum.ToObject(model.GetType(), confirmed is GXBitString bits ? bits.ToInteger() : Convert.ToInt64(confirmed));
            }

            if (confirmed is byte[] received)
            {
                if (received.Length == 6 && model is string && target is not GXDLMSData && target is not GXDLMSRegister
                    && !(target is GXDLMSCoAPSetup && index == 10)
                    && !(target is GXDLMSMBusPortSetup && index == 2))
                {
                    return string.Join(".", received);
                }
                if (received.Length != 0 && model is GXDateTime or DateTime or DateTimeOffset)
                {
                    return GXDLMSClient.ChangeType(received, model is GXDate ? DataType.Date : model is GXTime ? DataType.Time : DataType.DateTime);
                }
            }
            return confirmed;
        }
        if (target is GXDLMSMBusPortSetup mbusPort)
        {
            if (index == 2)
            {
                return (mbusPort.ProfileSelection ?? "0.0.0.0.0.0").Split('.').Select(byte.Parse).ToArray();
            }
            // GetValues returns the COSEM version instead of the M-Bus version.
            if (index == 8)
            {
                return mbusPort.MBusVersion;
            }
        }
        if (target is GXDLMSCoAPSetup coapSetup)
        {
            if (index == 2)
            {
                return (coapSetup.UdpReference?.LogicalName ?? "0.0.0.0.0.0").Split('.').Select(byte.Parse).ToArray();
            }
            if (index == 10)
            {
                return System.Text.Encoding.ASCII.GetBytes(coapSetup.CoAPUriPath ?? string.Empty);
            }
        }
        // PPP reports DataType.None until a user name exists, and GetValues
        // returns a combined display string. Its editor still needs both fields.
        if (target is GXDLMSPppSetup ppp && index == 5)
        {
            return new GXStructure { ppp.UserName ?? [], ppp.Password ?? [] };
        }

        if (target is GXDLMSAccount statusAccount && index == 4)
        {
            return new GXBitString((uint)statusAccount.CurrentCreditStatus, 8);
        }
        // XmlToValue does not support StringUTF8 nested in a structure. Preserve the
        // account currency's CLR string instead of round-tripping its wire XML.
        if (target is GXDLMSAccount account && index == 15)
        {
            return new GXStructure { account.Currency.Name ?? string.Empty, account.Currency.Scale, new GXEnum((byte)account.Currency.Unit) };
        }
        // Keep the typed timestamp: the XML decoder cannot read the hexadecimal
        // DateTime produced by DataToXml inside this structure.
        if (target is GXDLMSCoAPDiagnostic coap && index == 5)
        {
            return new GXStructure { coap.CaptureTime.AttributeId, coap.CaptureTime.TimeStamp! };
        }
        // Profile cells have already been decoded by the meter read. Re-encoding via capture metadata
        // would lose cells whose capture object's data type has not been read yet.
        if (target is GXDLMSProfileGeneric profile && index == 2)
        {
            var buffer = new GXArray();
            foreach (var row in profile.Buffer)
            {
                var cells = new GXStructure();
                foreach (var cell in row)
                {
                    cells.Add(cell);
                }

                buffer.Add(cells);
            }
            return buffer;
        }
        var original = target.GetValues().ElementAtOrDefault(index - 1);
        if (original is null && target.GetDataType(index) == DataType.Array)
        {
            return new GXArray();
        }

        if (target.GetDataType(index) is not (DataType.Array or DataType.Structure))
        {
            return original;
        }

        if (target is not IGXDLMSBase details)
        {
            return original;
        }

        var args = new ValueEventArgs(target, index, 0, null) { SkipMaxPduSize = true };
        var value = details.GetValue(new GXDLMSSettings(), args);
        if (args.Error != ErrorCode.Ok)
        {
            throw new InvalidOperationException($"Cannot display attribute {index}: {args.Error}.");
        }

        if (value is not byte[] bytes)
        {
            return value;
        }
        // XmlToValue reads decimal numeric values, matching ValueToXml's output.
        var translator = new GXDLMSTranslator(TranslatorOutputType.SimpleXml) { Hex = false };
        translator.DataToXml(bytes, out var xml);
        return GXDLMSTranslator.XmlToValue(xml);
    }
}
