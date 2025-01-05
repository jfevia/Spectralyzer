﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host.ViewModels;

public abstract class Item : ObservableObject
{
    public abstract string Title { get; }
}