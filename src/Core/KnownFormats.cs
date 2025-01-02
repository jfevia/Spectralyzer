// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public static class KnownFormats
{
    private static readonly HashSet<string> JsonMimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        MediaTypeNames.Application.Json,
        MediaTypeNames.Application.JsonLd,
        MediaTypeNames.Application.JsonPatch,
        MediaTypeNames.Application.JsonApi,
        MediaTypeNames.Application.JsonSequence,
        MediaTypeNames.Application.ProblemJson,
        MediaTypeNames.Application.JsonMergePatch,
        MediaTypeNames.Application.JsonSiren,
        MediaTypeNames.Application.JsonCollection
    };

    private static readonly HashSet<string> XmlMimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Application.Rss,
        MediaTypeNames.Application.Atom,
        MediaTypeNames.Application.MathML,
        MediaTypeNames.Application.Smil,
        MediaTypeNames.Application.Xaml,
        MediaTypeNames.Application.Soap,
        MediaTypeNames.Application.Wsdl,
        MediaTypeNames.Application.Xslt,
        MediaTypeNames.Application.XmlDtd,
        MediaTypeNames.Application.XmlPatch,
        MediaTypeNames.Application.ProblemXml,
        MediaTypeNames.Text.Xml,
        MediaTypeNames.Text.Html,
        MediaTypeNames.Image.Svg,
        MediaTypeNames.Document.OpenXmlWord,
        MediaTypeNames.Document.OpenXmlSpreadsheet,
        MediaTypeNames.Document.OpenXmlPresentation,
        MediaTypeNames.Document.OpenDocumentText,
        MediaTypeNames.Document.OpenDocumentSpreadsheet,
        MediaTypeNames.Ebook.EPUB
    };

    public static bool IsJson(string mimeType)
    {
        return JsonMimeTypes.Contains(mimeType);
    }

    public static bool IsXml(string mimeType)
    {
        return XmlMimeTypes.Contains(mimeType);
    }
}