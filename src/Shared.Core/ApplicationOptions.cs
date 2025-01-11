// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace Spectralyzer.Shared.Core;

public sealed class ApplicationOptions
{
    public string Environment { get; set; } = null!;
    public string ManufacturerName { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public Version Version { get; set; } = null!;
}