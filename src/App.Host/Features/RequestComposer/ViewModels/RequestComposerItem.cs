// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core.Http;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestComposerItem : Item
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RequestBodyViewModel _requestBody = new();
    private readonly RequestHeadersViewModel _requestHeaders = new();
    private readonly ResponseBodyViewModel _responseBody = new();
    private readonly ResponseHeadersViewModel _responseHeaders = new();
    private HttpMethod? _selectedMethod;
    private Uri? _url;

    public override string Title => "Request composer";

    public IEnumerable<HttpMethod> Methods { get; }
    public IEnumerable<RequestItemViewModel> RequestItems { get; }
    public ResponseDetailsViewModel ResponseDetails { get; }
    public IEnumerable<ResponseItemViewModel> ResponseItems { get; }

    public HttpMethod? SelectedMethod
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

    public RequestComposerItem(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        RequestItems =
        [
            _requestBody,
            _requestHeaders
        ];

        ResponseItems =
        [
            _responseBody,
            _responseHeaders
        ];

        ResponseDetails = new ResponseDetailsViewModel();

        Methods =
        [
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Delete,
            HttpMethod.Options,
            HttpMethod.Head,
            HttpMethod.Patch
        ];
        SelectedMethod = Methods.FirstOrDefault();

        SendRequestCommand = new AsyncRelayCommand(SendRequestAsync);
    }

    private static ResponseHeaderViewModel CreateResponseHeaderViewModel(string key, string value)
    {
        return new ResponseHeaderViewModel(key, value);
    }

    private HttpRequestMessage CreateHttpRequestMessage()
    {
        var httpRequestMessage = new HttpRequestMessage(_selectedMethod!, Url);

        foreach (var requestHeader in _requestHeaders.Items.Where(item => !string.IsNullOrEmpty(item.Key)))
        {
            httpRequestMessage.Headers.Add(requestHeader.Key!, requestHeader.Value);
        }

        httpRequestMessage.Content = !string.IsNullOrEmpty(_requestBody.Body)
            ? new StringContent(_requestBody.Body)
            : null;

        return httpRequestMessage;
    }

    private async Task ProcessHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        _responseHeaders.Items.Clear();

        ResponseDetails.StatusCode = (int)httpResponseMessage.StatusCode;
        ResponseDetails.StatusDescription = httpResponseMessage.ReasonPhrase;

        if (httpResponseMessage.RequestMessage?.Options.TryGetValue(HttpRequestOptionKeys.Elapsed, out var elapsed) == true)
        {
            ResponseDetails.Elapsed = elapsed;
        }

        ResponseDetails.ContentLength = httpResponseMessage.Content.Headers.ContentLength ?? 0;

        foreach (var httpResponseHeader in httpResponseMessage.Headers)
        {
            foreach (var httpResponseHeaderValue in httpResponseHeader.Value)
            {
                var responseHeaderViewModel = CreateResponseHeaderViewModel(httpResponseHeader.Key, httpResponseHeaderValue);
                _responseHeaders.Items.Add(responseHeaderViewModel);
            }
        }

        await _responseBody.ProcessHttpResponseMessageAsync(httpResponseMessage, cancellationToken);
    }

    private async Task SendRequestAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        var httpRequestMessage = CreateHttpRequestMessage();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        await ProcessHttpResponseMessageAsync(httpResponseMessage, cancellationToken);
    }
}