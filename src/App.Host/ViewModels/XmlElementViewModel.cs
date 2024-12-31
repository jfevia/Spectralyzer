// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Xml;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class XmlElementViewModel : XmlNodeViewModel
{
    private readonly Lazy<IEnumerable<XmlAttributeViewModel>> _attributeFactory;
    private readonly Lazy<IEnumerable<XmlObjectViewModel>> _nodeFactory;
    private readonly XmlElement _xmlElement;

    public IEnumerable<XmlAttributeViewModel> Attributes => _attributeFactory.Value;
    public string Name => _xmlElement.Name;
    public IEnumerable<XmlObjectViewModel> Nodes => _nodeFactory.Value;

    public XmlElementViewModel(XmlElement xmlElement, Func<XmlElement, IEnumerable<XmlObjectViewModel>> nodeFactory, Func<XmlElement, IEnumerable<XmlAttributeViewModel>> attributeFactory)
    {
        ArgumentNullException.ThrowIfNull(nodeFactory);

        _xmlElement = xmlElement;
        _nodeFactory = new Lazy<IEnumerable<XmlObjectViewModel>>(() => nodeFactory.Invoke(xmlElement));
        _attributeFactory = new Lazy<IEnumerable<XmlAttributeViewModel>>(() => attributeFactory.Invoke(xmlElement));
    }
}