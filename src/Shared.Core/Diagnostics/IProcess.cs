// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Shared.Core.Diagnostics;

public interface IProcess
{
    void Start(string filePath, string arguments);
}