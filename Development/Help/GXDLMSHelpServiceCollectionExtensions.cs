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

using Gurux.UI.Components;
using Gurux.UI.Components.Help;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gurux.DLMS.UI.Components.Help;

/// <summary>Registers COSEM documentation with the shared help service.</summary>
public static class GXDLMSHelpServiceCollectionExtensions
{
    /// <summary>Adds the COSEM provider and a default scoped help service without replacing host registrations.</summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The same collection for further registrations.</returns>
    public static IServiceCollection AddGXDLMSHelp(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IGXHelpProvider, GXDLMSObjectHelpProvider>());
        services.TryAddScoped<GXHelpService>();
        return services;
    }
}
