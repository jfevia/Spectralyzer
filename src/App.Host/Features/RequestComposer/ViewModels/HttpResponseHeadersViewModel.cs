// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpResponseHeadersViewModel : HttpMessageItemViewModel
{
    public override string Title => "Headers";
    public ObservableCollection<HttpResponseHeaderViewModel> Items { get; } = [];
}