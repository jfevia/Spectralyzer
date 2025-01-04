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
    private static class Format
    {
        public const string Fallback = "None";
        public const string Json = "JSON";
        public const string Xml = "XML";
    }

    private string? _body;
    private MonacoEditorController? _monacoEditorController;
    private string? _selectedFormat;

    public override string Title => "Body";

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value, OnBodyChanged);
    }

    public IEnumerable<string> Formats { get; }
    public ICommand InitializeCommand { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value, OnSelectedFormatChanged);
    }

    protected HttpMessageBodyViewModel()
    {
        Formats =
        [
            Format.Json,
            Format.Xml,
            Format.Fallback
        ];

        SelectedFormat = Formats.FirstOrDefault();

        InitializeCommand = new AsyncRelayCommand<WebView2>(InitializeAsync);
    }

    protected virtual Task InitializeEditorAsync(MonacoEditorController monacoEditorController, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
        await InitializeEditorAsync(_monacoEditorController, cancellationToken);
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