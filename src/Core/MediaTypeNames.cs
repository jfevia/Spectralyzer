// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.Core;

public static class MediaTypeNames
{
    public static class Application
    {
        public const string Atom = "application/atom+xml";
        public const string Json = "application/json";
        public const string JsonApi = "application/vnd.api+json";
        public const string JsonCollection = "application/vnd.collection+json";
        public const string JsonLd = "application/ld+json";
        public const string JsonMergePatch = "application/merge-patch+json";
        public const string JsonPatch = "application/json-patch+json";
        public const string JsonSequence = "application/json-seq";
        public const string JsonSiren = "application/vnd.siren+json";
        public const string MathML = "application/mathml+xml";
        public const string ProblemJson = "application/problem+json";
        public const string ProblemXml = "application/problem+xml";
        public const string Rss = "application/rss+xml";
        public const string Smil = "application/smil+xml";
        public const string Soap = "application/soap+xml";
        public const string Wsdl = "application/wsdl+xml";
        public const string Xaml = "application/xaml+xml";
        public const string Xml = "application/xml";
        public const string XmlDtd = "application/xml-dtd";
        public const string XmlPatch = "application/xml-patch+xml";
        public const string Xslt = "application/xslt+xml";
    }

    public static class Document
    {
        public const string OpenDocumentSpreadsheet = "application/vnd.oasis.opendocument.spreadsheet+xml";
        public const string OpenDocumentText = "application/vnd.oasis.opendocument.text+xml";
        public const string OpenXmlPresentation = "application/vnd.ms-powerpoint.presentation+xml";
        public const string OpenXmlSpreadsheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet+xml";
        public const string OpenXmlWord = "application/vnd.openxmlformats-officedocument.wordprocessingml.document+xml";
    }

    public static class Ebook
    {
        public const string EPUB = "application/epub+zip";
    }

    public static class Image
    {
        public const string Svg = "image/svg+xml";
    }

    public static class Text
    {
        public const string Html = "text/html";
        public const string Xml = "text/xml";
    }
}