// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Xml;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class XmlDocumentViewModel : XmlNodeViewModel
{
    private readonly Lazy<IEnumerable<XmlObjectViewModel>> _nodeFactory;
    private readonly XmlDocument _xmlDocument;

    public IEnumerable<XmlObjectViewModel> Nodes => _nodeFactory.Value;

    public XmlDocumentViewModel(XmlDocument xmlDocument)
    {
        _xmlDocument = xmlDocument ?? throw new ArgumentNullException(nameof(xmlDocument));
        _nodeFactory = new Lazy<IEnumerable<XmlObjectViewModel>>(GetNodes);
    }

    private IEnumerable<XmlObjectViewModel> GetNodes()
    {
        return XmlFactory.ParseElement(_xmlDocument.DocumentElement);
    }
}