// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Net.Http;

namespace Spectralyzer.Core.Http;

public static class HttpRequestOptionKeys
{
    public static readonly HttpRequestOptionsKey<TimeSpan> Elapsed = new("Elapsed");
}