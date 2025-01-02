// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Net.Http;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MimeKit;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core;

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

    private async Task SendRequestAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var httpRequestMessage = new HttpRequestMessage(_selectedMethod!, Url);

        foreach (var requestHeader in _requestHeaders.Items.Where(item => !string.IsNullOrEmpty(item.Key)))
        {
            httpRequestMessage.Headers.Add(requestHeader.Key!, requestHeader.Value);
        }

        httpRequestMessage.Content = !string.IsNullOrEmpty(_requestBody.Body) ? new StringContent(_requestBody.Body) : null;

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        _responseHeaders.Items.Clear();

        foreach (var httpResponseHeader in httpResponseMessage.Headers)
        {
            foreach (var httpResponseHeaderValue in httpResponseHeader.Value)
            {
                _responseHeaders.Items.Add(new ResponseHeaderViewModel(httpResponseHeader.Key, httpResponseHeaderValue));
            }
        }

        _responseBody.Body = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        var result = GetContentType();
        if (result.IsMatch)
        {
            _responseBody.SelectedFormat = GetSelectedFormat(result.ContentType);
        }
        else
        {
            _responseBody.SelectedFormat = "None";
        }

        return;

        (bool IsMatch, ContentType? ContentType) GetContentType()
        {
            if (httpResponseMessage.Content.Headers.ContentType is null)
            {
                return (false, null);
            }

            return HeaderHelper.FindContentType(httpResponseMessage.Content.Headers.ContentType);
        }

        string GetSelectedFormat(ContentType? contentType)
        {
            if (contentType is null)
            {
                return "None";
            }

            if (KnownFormats.IsJson(contentType.MimeType))
            {
                return "JSON";
            }

            if (KnownFormats.IsXml(contentType.MimeType))
            {
                return "XML";
            }

            return "None";
        }
    }
}