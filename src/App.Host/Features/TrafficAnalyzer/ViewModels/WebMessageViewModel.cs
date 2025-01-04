// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using Spectralyzer.App.Host.Controllers;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public abstract class WebMessageViewModel : ObservableObject
{
    private static class Format
    {
        public const string Fallback = "None";
        public const string Json = "JSON";
        public const string Xml = "XML";
    }

    private string? _body;
    private string? _httpView;
    private MonacoEditorController? _monacoEditorController;
    private string? _selectedFormat;
    private Version? _version;

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value, OnBodyChanged);
    }

    public IEnumerable<string> Formats { get; }
    public ObservableCollection<WebHeader> Headers { get; } = [];

    public string? HttpView
    {
        get => _httpView;
        private set => SetProperty(ref _httpView, value);
    }

    public ICommand InitializeCommand { get; }

    public Guid RequestId { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value, OnSelectedFormatChanged);
    }

    public Version? Version
    {
        get => _version;
        protected set => SetProperty(ref _version, value);
    }

    protected WebMessageViewModel(Guid requestId)
    {
        RequestId = requestId;

        Formats =
        [
            Format.Json,
            Format.Xml,
            Format.Fallback
        ];
        SelectedFormat = Formats.LastOrDefault();

        InitializeCommand = new AsyncRelayCommand<WebView2>(InitializeAsync);
    }

    protected virtual void OnGeneratingHttpViewBody(StringBuilder stringBuilder)
    {
        if (string.IsNullOrEmpty(_body))
        {
            return;
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine(_body);
    }

    protected virtual void OnGeneratingHttpViewHeaders(StringBuilder stringBuilder)
    {
        foreach (var header in Headers.OrderBy(header => header.Key))
        {
            foreach (var headerValue in header.Values)
            {
                stringBuilder.AppendLine($"{header.Key}: {headerValue}");
            }
        }
    }

    private string GenerateHttpView()
    {
        var stringBuilder = new StringBuilder();

        OnGeneratingHttpViewHeaders(stringBuilder);
        OnGeneratingHttpViewBody(stringBuilder);

        return stringBuilder.ToString();
    }

    private async Task InitializeAsync(WebView2? obj, CancellationToken cancellationToken)
    {
        if (obj is null)
        {
            return;
        }

        _monacoEditorController = new MonacoEditorController(obj);

        var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\MonacoEditor\Index.html");
        var sourceUri = new Uri(source);

        await _monacoEditorController.InitializeAsync(sourceUri, cancellationToken);
        OnSelectedFormatChanged(_selectedFormat);
        OnBodyChanged(_body);
        await _monacoEditorController.SetIsReadOnlyAsync(true);
    }

    private async void OnBodyChanged(string? value)
    {
        try
        {
            HttpView = GenerateHttpView();

            if (_monacoEditorController is not null && value is not null)
            {
                await _monacoEditorController.SetContentAsync(HttpView);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void OnSelectedFormatChanged(string? value)
    {
        try
        {
            if (_monacoEditorController is not null && value is not null)
            {
                await _monacoEditorController.SetLanguageAsync(value);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}

public abstract class WebMessageViewModel<TMessage> : WebMessageViewModel
    where TMessage : WebMessage
{
    protected WebMessageViewModel(Guid requestId)
        : base(requestId)
    {
    }

    public virtual void ProcessMessage(TMessage message)
    {
        Headers.Clear();

        foreach (var header in message.Headers)
        {
            Headers.Add(header);
        }

        Body = message.BodyAsString;
        Version = message.Version;
    }
}