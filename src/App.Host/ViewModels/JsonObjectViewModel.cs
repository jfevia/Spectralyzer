// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace Spectralyzer.App.Host.ViewModels;

public abstract class JsonObjectViewModel : ObservableObject
{
    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }
}