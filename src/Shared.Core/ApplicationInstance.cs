// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Shared.Core;

public static class ApplicationInstance
{
    public static string Environment { get; set; } = null!;
    public static string ManufacturerName { get; set; } = null!;
    public static string ProductName { get; set; } = null!;
    public static string ProductVersion { get; set; } = null!;
}