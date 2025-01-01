// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.App.Host.ViewModels;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestComposerItem : Item
{
    private string? _selectedMethod;
    private Uri? _url;

    public override string Title => "Request composer";

    public IEnumerable<RequestItemViewModel> RequestItems { get; }
    public IEnumerable<ResponseItemViewModel> ResponseItems { get; }

    public IEnumerable<string> Methods { get; }

    public string? SelectedMethod
    {
        get => _selectedMethod;
        set => SetProperty(ref _selectedMethod, value);
    }

    public ICommand SendRequestCommand { get; }

    public Uri? Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    public RequestComposerItem()
    {
        RequestItems =
        [
            new RequestParametersViewModel(),
            new RequestBodyViewModel(),
            new RequestAuthViewModel(),
            new RequestHeadersViewModel()
        ];
        
        ResponseItems = 
        [
            new ResponsePreviewViewModel(),
            new ResponseHeadersViewModel(),
            new CookiesViewModel()
        ];

        Methods =
        [
            "GET",
            "POST",
            "PUT",
            "DELETE",
            "OPTIONS",
            "HEAD",
            "PATCH"
        ];
        SelectedMethod = Methods.FirstOrDefault();

        SendRequestCommand = new AsyncRelayCommand(SendRequestAsync);
    }

    private Task SendRequestAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}