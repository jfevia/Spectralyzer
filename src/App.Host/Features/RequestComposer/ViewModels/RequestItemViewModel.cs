// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public abstract class RequestItemViewModel : ObservableObject
{
    public abstract string Title { get; }
}