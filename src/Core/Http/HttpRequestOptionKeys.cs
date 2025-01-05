// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core.Http;

public static class HttpRequestOptionKeys
{
    public static readonly HttpRequestOptionsKey<TimeSpan> Elapsed = new("Elapsed");
}