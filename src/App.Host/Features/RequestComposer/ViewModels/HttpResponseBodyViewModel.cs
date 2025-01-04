// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpResponseBodyViewModel : HttpMessageBodyViewModel
{
    public override string Title => "Preview";

    public HttpResponseBodyViewModel()
    {
        IsReadOnly = true;
    }

    public async Task ProcessHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        Body = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
    }
}