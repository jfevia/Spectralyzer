// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Spectralyzer.Shared.UI.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public abstract class HttpMessageItemViewModel : ObservableObject
{
    public abstract string Title { get; }
}