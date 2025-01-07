// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Spectralyzer.Shared.UI;
using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.App.Host.ViewModels;

public abstract class Item : ObservableObject
{
    public abstract string Title { get; }
}