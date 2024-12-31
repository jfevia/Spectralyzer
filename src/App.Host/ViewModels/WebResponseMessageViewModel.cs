// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class WebResponseMessageViewModel : WebMessageViewModel
{
    private readonly WebResponseMessage _webResponseMessage;

    public HttpStatusCode HttpStatusCode => _webResponseMessage.HttpStatusCode;

    public WebResponseMessageViewModel(WebResponseMessage webResponseMessage)
        : base(webResponseMessage)
    {
        _webResponseMessage = webResponseMessage ?? throw new ArgumentNullException(nameof(webResponseMessage));
    }
}