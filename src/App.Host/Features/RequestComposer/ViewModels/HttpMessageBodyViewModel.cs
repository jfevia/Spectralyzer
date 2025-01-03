// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using Spectralyzer.App.Host.Controllers;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public abstract class HttpMessageBodyViewModel : HttpMessageItemViewModel
{
    protected static class Format
    {
        public const string Fallback = "None";
        public const string Json = "JSON";
        public const string Xml = "XML";
    }

    private string? _body;
    private string? _selectedFormat;

    public override string Title => "Body";

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value, OnBodyChanged);
    }

    public IEnumerable<string> Formats { get; }
    public ICommand InitializeCommand { get; }
    public ICommand InitializeEditorCommand { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value, OnSelectedFormatChanged);
    }

    private MonacoEditorController? _monacoEditorController;

    protected HttpMessageBodyViewModel()
    {
        Formats =
        [
            Format.Json,
            Format.Xml,
            Format.Fallback
        ];

        SelectedFormat = Formats.FirstOrDefault();

        InitializeCommand = new RelayCommand<WebView2>(Initialize);
        InitializeEditorCommand = new AsyncRelayCommand<WebView2>(InitializeEditorAsync);
    }

    private async Task InitializeEditorAsync(WebView2? obj, CancellationToken cancellationToken)
    {
        if (_monacoEditorController is null || obj is null)
        {
            return;
        }

        await _monacoEditorController.InitializeAsync(cancellationToken);
        OnSelectedFormatChanged(_selectedFormat);
        OnBodyChanged(_body);
        await InitializeEditorAsync(_monacoEditorController, cancellationToken);
    }

    protected virtual Task InitializeEditorAsync(MonacoEditorController monacoEditorController, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void Initialize(WebView2? obj)
    {
        if (obj is null)
        {
            return;
        }

        _monacoEditorController = new MonacoEditorController(obj);

        var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\MonacoEditor\Index.html");
        var sourceUri = new Uri(source);
        _monacoEditorController.NavigateTo(sourceUri);
    }

    private async void OnBodyChanged(string? value)
    {
        try
        {
            if (_monacoEditorController is not null && value is not null)
            {
                await _monacoEditorController.SetContentAsync(value);
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