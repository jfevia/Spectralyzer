// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host;

public abstract class ObservableObject : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    protected void SetProperty<T>(ref T field, T value, Action<T>? onChanged = null, string? propertyName = null)
    {
        if (SetProperty(ref field, value, propertyName))
        {
            onChanged?.Invoke(value);
        }
    }
}