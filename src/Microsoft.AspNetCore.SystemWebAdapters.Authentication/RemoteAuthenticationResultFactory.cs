// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.SystemWebAdapters.Authentication;

/// <summary>
/// Factory type for generating remote authentication results from HTTP responses
/// from a remote app.
/// </summary>
public class RemoteAuthenticationResultFactory : IAuthenticationResultFactory
{
    private readonly ILogger<RemoteAuthenticationResultFactory> _logger;

    public RemoteAuthenticationResultFactory(ILogger<RemoteAuthenticationResultFactory> logger)
    {
        _logger = logger;
    }

    public async Task<RemoteAuthenticationResult> CreateRemoteAuthenticationResultAsync(HttpResponseMessage response, RemoteAppAuthenticationOptions options)
    {
        RemoteAuthenticationResult? ret = null;

        // If the result has a 200 status code, attempt to deserialize the ClaimsPrincipal
        if (response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                using var responseContent = await response.Content.ReadAsStreamAsync();
                using var reader = new BinaryReader(responseContent);
                ret = new RemoteAuthenticationResult(new ClaimsPrincipal(reader), (int)response.StatusCode);
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Failed to deserialize remote authentication response as a claims principal despite success status code");
            }
        }

        // If the remote authentication result hasn't yet been created, create it without a claims principal
        if (ret is null)
        {
            ret = new RemoteAuthenticationResult(null, (int)response.StatusCode);
        }

        // Copy expected response headers
        foreach (var responseHeader in response.Headers)
        {
            if (!options.ResponseHeadersToForward.Any() || options.ResponseHeadersToForward.Contains(responseHeader.Key))
            {
                ret.ResponseHeaders.Add(responseHeader.Key, responseHeader.Value.ToArray());
            }
        }

        return ret;
    }
}
