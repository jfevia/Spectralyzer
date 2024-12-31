// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Xml;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class XmlAttributeViewModel : XmlObjectViewModel
{
    private readonly XmlAttribute _xmlAttribute;

    public string Name => _xmlAttribute.Name;
    public string Value => _xmlAttribute.Value;

    public XmlAttributeViewModel(XmlAttribute xmlAttribute)
    {
        _xmlAttribute = xmlAttribute;
    }
}