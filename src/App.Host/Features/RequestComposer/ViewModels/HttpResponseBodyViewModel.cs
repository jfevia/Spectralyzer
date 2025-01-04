// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;
using Spectralyzer.App.Host.Controllers;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpResponseBodyViewModel : HttpMessageBodyViewModel
{
    public override string Title => "Preview";

    protected override async Task InitializeEditorAsync(MonacoEditorController monacoEditorController, CancellationToken cancellationToken)
    {
        await monacoEditorController.SetIsReadOnlyAsync(true);
        await base.InitializeEditorAsync(monacoEditorController, cancellationToken);
    }

    public async Task ProcessHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        Body = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
    }
}