// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core.Http;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class HttpRequestComposerItem : Item
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpRequestBodyViewModel _httpRequestBody = new();
    private readonly HttpRequestHeadersViewModel _httpRequestHeaders = new();
    private readonly HttpResponseBodyViewModel _httpResponseBody = new();
    private readonly HttpResponseHeadersViewModel _httpResponseHeaders = new();
    private HttpMethod? _selectedMethod;
    private Uri? _url;

    public override string Title => "Request composer";

    public IEnumerable<HttpMethod> Methods { get; }
    public IEnumerable<HttpMessageItemViewModel> RequestItems { get; }
    public HttpResponseDetailsViewModel HttpResponseDetails { get; }
    public IEnumerable<HttpMessageItemViewModel> ResponseItems { get; }

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

    public HttpRequestComposerItem(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        RequestItems =
        [
            _httpRequestBody,
            _httpRequestHeaders
        ];

        ResponseItems =
        [
            _httpResponseBody,
            _httpResponseHeaders
        ];

        HttpResponseDetails = new HttpResponseDetailsViewModel();

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

    private static HttpResponseHeaderViewModel CreateResponseHeaderViewModel(string key, string value)
    {
        return new HttpResponseHeaderViewModel(key, value);
    }

    private HttpRequestMessage CreateHttpRequestMessage()
    {
        var httpRequestMessage = new HttpRequestMessage(_selectedMethod!, Url);

        foreach (var requestHeader in _httpRequestHeaders.Items.Where(item => !string.IsNullOrEmpty(item.Key)))
        {
            httpRequestMessage.Headers.Add(requestHeader.Key!, requestHeader.Value);
        }

        httpRequestMessage.Content = !string.IsNullOrEmpty(_httpRequestBody.Body)
            ? new StringContent(_httpRequestBody.Body)
            : null;

        return httpRequestMessage;
    }

    private async Task ProcessHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        _httpResponseHeaders.Items.Clear();

        HttpResponseDetails.StatusCode = (int)httpResponseMessage.StatusCode;
        HttpResponseDetails.StatusDescription = httpResponseMessage.ReasonPhrase;

        if (httpResponseMessage.RequestMessage?.Options.TryGetValue(HttpRequestOptionKeys.Elapsed, out var elapsed) == true)
        {
            HttpResponseDetails.Elapsed = elapsed;
        }

        HttpResponseDetails.ContentLength = httpResponseMessage.Content.Headers.ContentLength ?? 0;

        foreach (var httpResponseHeader in httpResponseMessage.Headers)
        {
            foreach (var httpResponseHeaderValue in httpResponseHeader.Value)
            {
                var responseHeaderViewModel = CreateResponseHeaderViewModel(httpResponseHeader.Key, httpResponseHeaderValue);
                _httpResponseHeaders.Items.Add(responseHeaderViewModel);
            }
        }

        await _httpResponseBody.ProcessHttpResponseMessageAsync(httpResponseMessage, cancellationToken);
    }

    private async Task SendRequestAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        var httpRequestMessage = CreateHttpRequestMessage();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        await ProcessHttpResponseMessageAsync(httpResponseMessage, cancellationToken);
    }
}