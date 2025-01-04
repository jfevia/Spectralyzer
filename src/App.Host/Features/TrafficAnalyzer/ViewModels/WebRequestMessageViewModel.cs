// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Text;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class WebRequestMessageViewModel : WebMessageViewModel<WebRequestMessage>
{
    public string HttpMethod { get; }
    public Uri RequestUri { get; }

    public WebRequestMessageViewModel(Guid requestId, Uri requestUri, string method)
        : base(requestId)
    {
        RequestUri = requestUri;
        HttpMethod = method ?? throw new ArgumentNullException(nameof(method));
    }

    protected override void OnGeneratingHttpViewHeaders(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{HttpMethod} {RequestUri.PathAndQuery}");
        base.OnGeneratingHttpViewHeaders(stringBuilder);
    }
}