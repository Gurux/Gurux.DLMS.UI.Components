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
using Gurux.DLMS.Objects.Italy;
namespace Gurux.DLMS.UI.Components.Objects;
/// <summary>Explicit view references also preserve components in trimmed mobile applications.</summary>
public static class GXDLMSObjectViewRegistry
{
    private static readonly IReadOnlyDictionary<Type, Type> Views = new Dictionary<Type, Type>
    {
        [typeof(GXDLMSAccount)] = typeof(GXAccountView),
        [typeof(GXDLMSActionSchedule)] = typeof(GXActionScheduleView),
        [typeof(GXDLMSActivityCalendar)] = typeof(GXActivityCalendarView),
        [typeof(GXDLMSArbitrator)] = typeof(GXArbitratorView),
        [typeof(GXDLMSArrayManager)] = typeof(GXArrayManagerView),
        [typeof(GXDLMSAssociationLogicalName)] = typeof(GXAssociationLogicalNameView),
        [typeof(GXDLMSAssociationShortName)] = typeof(GXAssociationShortNameView),
        [typeof(GXDLMSAutoAnswer)] = typeof(GXDLMSAutoAnswerView),
        [typeof(GXDLMSAutoConnect)] = typeof(GXDLMSAutoConnectView),
        [typeof(GXDLMSCharge)] = typeof(GXChargeView),
        [typeof(GXDLMSClock)] = typeof(GXClockView),
        [typeof(GXDLMSCoAPDiagnostic)] = typeof(GXCoAPDiagnosticView),
        [typeof(GXDLMSCoAPSetup)] = typeof(GXCoAPSetupView),
        [typeof(GXDLMSCommunicationPortProtection)] = typeof(GXCommunicationPortProtectionView),
        [typeof(GXDLMSCompactData)] = typeof(GXCompactDataView),
        [typeof(GXDLMSCredit)] = typeof(GXCreditView),
        [typeof(GXDLMSData)] = typeof(GXDLMSDataView),
        [typeof(GXDLMSDataProtection)] = typeof(GXDataProtectionView),
        [typeof(GXDLMSDemandRegister)] = typeof(GXDemandRegisterView),
        [typeof(GXDLMSDisconnectControl)] = typeof(GXDisconnectControlView),
        [typeof(GXDLMSExtendedRegister)] = typeof(GXExtendedRegisterView),
        [typeof(GXDLMSFunctionControl)] = typeof(GXFunctionControlView),
        [typeof(GXDLMSG3Plc6LoWPan)] = typeof(GXDLMSG3Plc6LoWPanView),
        [typeof(GXDLMSG3PlcMacLayerCounters)] = typeof(GXDLMSG3PlcMacLayerCountersView),
        [typeof(GXDLMSG3PlcMacSetup)] = typeof(GXDLMSG3PlcMacSetupView),
        [typeof(GXDLMSGprsSetup)] = typeof(GXDLMSGprsSetupView),
        [typeof(GXDLMSGSMDiagnostic)] = typeof(GXDLMSGSMDiagnosticView),
        [typeof(GXDLMSHdlcSetup)] = typeof(GXDLMSHdlcSetupView),
        [typeof(GXDLMSIECLocalPortSetup)] = typeof(GXDLMSIECLocalPortSetupView),
        [typeof(GXDLMSIecTwistedPairSetup)] = typeof(GXDLMSIecTwistedPairSetupView),
        [typeof(GXDLMSImageTransfer)] = typeof(GXImageTransferView),
        [typeof(GXDLMSIp4Setup)] = typeof(GXDLMSIp4SetupView),
        [typeof(GXDLMSIp6Setup)] = typeof(GXDLMSIp6SetupView),
        [typeof(GXDLMSLimiter)] = typeof(GXLimiterView),
        [typeof(GXDLMSLlcSscsSetup)] = typeof(GXDLMSLlcSscsSetupView),
        [typeof(GXDLMSMacAddressSetup)] = typeof(GXDLMSMacAddressSetupView),
        [typeof(GXDLMSMBusClient)] = typeof(GXMBusClientView),
        [typeof(GXDLMSMBusDiagnostic)] = typeof(GXDLMSMBusDiagnosticView),
        [typeof(GXDLMSMBusMasterPortSetup)] = typeof(GXDLMSMBusMasterPortSetupView),
        [typeof(GXDLMSMBusPortSetup)] = typeof(GXMBusPortSetupView),
        [typeof(GXDLMSMBusSlavePortSetup)] = typeof(GXDLMSMBusSlavePortSetupView),
        [typeof(GXDLMSModemConfiguration)] = typeof(GXDLMSModemConfigurationView),
        [typeof(GXDLMSNtpSetup)] = typeof(GXDLMSNtpSetupView),
        [typeof(GXDLMSParameterMonitor)] = typeof(GXParameterMonitorView),
        [typeof(GXDLMSPppSetup)] = typeof(GXDLMSPppSetupView),
        [typeof(GXDLMSPrimeNbOfdmPlcApplicationsIdentification)] = typeof(GXDLMSPrimeNbOfdmPlcApplicationsIdentificationView),
        [typeof(GXDLMSPrimeNbOfdmPlcMacCounters)] = typeof(GXDLMSPrimeNbOfdmPlcMacCountersView),
        [typeof(GXDLMSPrimeNbOfdmPlcMacFunctionalParameters)] = typeof(GXDLMSPrimeNbOfdmPlcMacFunctionalParametersView),
        [typeof(GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData)] = typeof(GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationDataView),
        [typeof(GXDLMSPrimeNbOfdmPlcMacSetup)] = typeof(GXDLMSPrimeNbOfdmPlcMacSetupView),
        [typeof(GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters)] = typeof(GXDLMSPrimeNbOfdmPlcPhysicalLayerCountersView),
        [typeof(GXDLMSProfileGeneric)] = typeof(GXProfileGenericView),
        [typeof(GXDLMSPushSetup)] = typeof(GXPushSetupView),
        [typeof(GXDLMSRegister)] = typeof(GXDLMSRegisterView),
        [typeof(GXDLMSRegisterActivation)] = typeof(GXRegisterActivationView),
        [typeof(GXDLMSRegisterMonitor)] = typeof(GXRegisterMonitorView),
        [typeof(GXDLMSSapAssignment)] = typeof(GXSapAssignmentView),
        [typeof(GXDLMSSchedule)] = typeof(GXScheduleView),
        [typeof(GXDLMSScriptTable)] = typeof(GXScriptTableView),
        [typeof(GXDLMSSecuritySetup)] = typeof(GXSecuritySetupView),
        [typeof(GXDLMSSFSKActiveInitiator)] = typeof(GXDLMSSFSKActiveInitiatorView),
        [typeof(GXDLMSSFSKMacCounters)] = typeof(GXDLMSSFSKMacCountersView),
        [typeof(GXDLMSSFSKMacSynchronizationTimeouts)] = typeof(GXDLMSSFSKMacSynchronizationTimeoutsView),
        [typeof(GXDLMSSFSKPhyMacSetUp)] = typeof(GXDLMSSFSKPhyMacSetUpView),
        [typeof(GXDLMSSpecialDaysTable)] = typeof(GXSpecialDaysTableView),
        [typeof(GXDLMSTariffPlan)] = typeof(GXTariffPlanView),
        [typeof(GXDLMSTcpUdpSetup)] = typeof(GXDLMSTcpUdpSetupView),
        [typeof(GXDLMSTokenGateway)] = typeof(GXTokenGatewayView),
        [typeof(GXDLMSUtilityTables)] = typeof(GXUtilityTablesView),
        [typeof(GXDLMSWirelessModeQchannel)] = typeof(GXDLMSWirelessModeQchannelView),
    };
    /// <summary>Finds the view component for an object, using the generic view when supported.</summary>
    /// <param name="target">The object to display.</param>
    /// <returns>The matching component type, or null when no view is available.</returns>
    public static Type? GetViewType(GXDLMSObject target)
    {
        if (target is GXDLMSData && target.LogicalName == GXDLMSUnixTimeView.UnixTimeLogicalName)
        {
            return typeof(GXDLMSUnixTimeView);
        }

        return Views.GetValueOrDefault(target.GetType()) ?? (target is IGXDLMSBase ? typeof(GXGenericObjectView) : null);
    }
}
