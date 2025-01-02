// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http.Headers;
using MimeKit;

namespace Spectralyzer.Core;

public static class HeaderHelper
{
    public static (bool IsMatch, ContentType? ContentType) FindContentType(string key, string value)
    {
        using var stream = new MemoryStream();
        using var textWriter = new StreamWriter(stream);
        textWriter.Write($"{key}: {value}");
        textWriter.Flush();
        stream.Position = 0;
        return FindContentType(stream);
    }

    public static (bool IsMatch, ContentType? ContentType) FindContentType(MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        return FindContentType("Content-Type", mediaTypeHeaderValue.ToString());
    }

    private static (bool IsMatch, ContentType? ContentType) FindContentType(Stream stream)
    {
        var mimeParser = new MimeParser(stream, true);
        var headerList = mimeParser.ParseHeaders();

        var contentTypeHeader = headerList.FirstOrDefault(s => s.Id == HeaderId.ContentType);
        if (contentTypeHeader is null)
        {
            return (IsMatch: false, ContentType: null);
        }

        var contentType = ContentType.Parse(contentTypeHeader.Value);
        return (IsMatch: true, ContentType: contentType);
    }
}