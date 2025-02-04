﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Net;
using Titanium.Web.Proxy.Models;

namespace Spectralyzer.Core;

public sealed class WebProxyEndpoint
{
    public bool DecryptSsl => Object.DecryptSsl;

    public IPAddress IPAddress => Object.IpAddress;

    public int Port => Object.Port;

    internal ExplicitProxyEndPoint Object { get; }

    internal WebProxyEndpoint(ExplicitProxyEndPoint explicitProxyEndPoint)
    {
        Object = explicitProxyEndPoint ?? throw new ArgumentNullException(nameof(explicitProxyEndPoint));
    }
}