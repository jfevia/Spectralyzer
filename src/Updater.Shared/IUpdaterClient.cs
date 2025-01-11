﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace Spectralyzer.Updater.Shared;

public interface IUpdaterClient
{
    Task StartAsync(CancellationToken cancellationToken);
}