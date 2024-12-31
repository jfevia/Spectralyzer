// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Xml;

namespace Spectralyzer.App.Host.ViewModels;

public static class XmlFactory
{
    public static IEnumerable<XmlObjectViewModel> ParseElement(XmlElement? xmlElement)
    {
        if (xmlElement is null)
        {
            yield break;
        }

        foreach (XmlNode childNode in xmlElement.ChildNodes)
        {
            switch (childNode)
            {
                case XmlElement elementNode:
                    yield return CreateXmlElementViewModel(elementNode);
                    break;

                case XmlText:
                case XmlCDataSection:
                    yield return CreateXmlPrimitiveViewModel(childNode);
                    break;
            }
        }
    }

    private static XmlAttributeViewModel CreateXmlAttributeViewModel(XmlAttribute xmlAttribute)
    {
        return new XmlAttributeViewModel(xmlAttribute);
    }

    private static XmlElementViewModel CreateXmlElementViewModel(XmlElement childNode)
    {
        return new XmlElementViewModel(childNode, ParseNode, ParseAttributes);
    }

    private static XmlPrimitiveViewModel CreateXmlPrimitiveViewModel(XmlNode xmlNode)
    {
        return new XmlPrimitiveViewModel(xmlNode.Value);
    }

    private static IEnumerable<XmlAttributeViewModel> ParseAttributes(XmlElement xmlElement)
    {
        return xmlElement.Attributes.Cast<XmlAttribute>().Select(CreateXmlAttributeViewModel);
    }

    private static IEnumerable<XmlObjectViewModel> ParseNode(XmlElement? xmlElement)
    {
        if (xmlElement is null)
        {
            yield break;
        }

        foreach (var xmlObject in ParseElement(xmlElement))
        {
            yield return xmlObject;
        }
    }
}