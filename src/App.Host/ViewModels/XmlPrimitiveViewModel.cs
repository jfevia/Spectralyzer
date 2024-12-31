// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host.ViewModels;

public sealed class XmlPrimitiveViewModel : XmlObjectViewModel
{
    public string? Value { get; }

    public XmlPrimitiveViewModel(string? value)
    {
        Value = value;
    }
}