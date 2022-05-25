// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.SystemWebAdapters.Authentication;

namespace Microsoft.AspNetCore.SystemWebAdapters;

/// <summary>
/// Helper methods for registering remote authentication endpoints.
/// </summary>
public static class RemoteAuthenticationExtensions
{
    /// <summary>
    /// Adds the remote authentication module to System.Web adapter configuration.
    /// </summary>
    /// <param name="builder">The System.Web adapter builder to modify.</param>
    /// <param name="configureRemoteAuthentication">Configuration to use when registering the remote authentication module.</param>
    /// <returns>The System.Web adapter builder updated to include the remote authentication module.</returns>
    public static ISystemWebAdapterBuilder AddRemoteAuthentication(this ISystemWebAdapterBuilder builder, Action<RemoteAppAuthenticationOptions> configureRemoteAuthentication)
    {
        var options = new RemoteAppAuthenticationOptions();
        configureRemoteAuthentication(options);

        builder.Modules.Add(new RemoteAuthenticationModule(options));
        return builder;
    }
}
