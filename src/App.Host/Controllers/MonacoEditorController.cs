// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;
using System.Web;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Spectralyzer.App.Host.Controllers;

public sealed class MonacoEditorController
{
    private const string EditorContainerSelector = "#root";
    private const string EditorObject = "MonacoEditor";
    private readonly WebView2 _webView2;
    private bool _isInitialized;

    public MonacoEditorController(WebView2 webView2)
    {
        _webView2 = webView2 ?? throw new ArgumentNullException(nameof(webView2));
    }

    public async Task InitializeAsync(Uri source)
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        var navigationTask = WaitForNavigationCompletedAsync();
        _webView2.NavigationCompleted += OnNavigationCompleted;
        await _webView2.EnsureCoreWebView2Async();
        _webView2.Source = source;
        await navigationTask;
    }

    public async Task SetContentAsync(string? contents)
    {
        var escapedContents = !string.IsNullOrEmpty(contents) ? HttpUtility.JavaScriptStringEncode(contents) : contents;
        await _webView2.ExecuteScriptAsync($"{EditorObject}.setValue(\"{escapedContents}\");");
    }

    public async Task SetIsReadOnlyAsync(bool value)
    {
        await _webView2.ExecuteScriptAsync($"{EditorObject}.updateOptions({{ readOnly: {value.ToString().ToLowerInvariant()} }});");
    }

    public async Task SetLanguageAsync(string languageId)
    {
        await _webView2.ExecuteScriptAsync($"monaco.editor.setModelLanguage({EditorObject}.getModel(), \"{languageId.ToLowerInvariant()}\");");
    }

    private Task<bool> WaitForNavigationCompletedAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        EventHandler<CoreWebView2NavigationCompletedEventArgs>? handler = null;
        handler = (_, e) =>
        {
            _webView2.NavigationCompleted -= handler;

            if (e.IsSuccess)
            {
                taskCompletionSource.SetResult(true);
            }
            else
            {
                taskCompletionSource.SetException(new Exception($"Navigation failed with error: {e.WebErrorStatus}"));
            }
        };

        _webView2.NavigationCompleted += handler;
        return taskCompletionSource.Task;
    }

    private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        const string uiThemeName = "_default";

        try
        {
            _webView2.NavigationCompleted -= OnNavigationCompleted;

            _ = await _webView2.ExecuteScriptAsync(
                $$"""
                  const {{EditorObject}} = monaco.editor.create(document.querySelector('{{EditorContainerSelector}}'));
                  window.onresize = () => {{{EditorObject}}.layout();}
                  """
            );

            _ = await _webView2.ExecuteScriptAsync(
                $$$"""
                   monaco.editor.defineTheme('{{{uiThemeName}}}', {
                       base: 'vs-dark',
                       inherit: true,
                       rules: [{ background: 'FFFFFF00' }],
                       colors: {'editor.background': '#FFFFFF00','minimap.background': '#FFFFFF00',}});
                   monaco.editor.setTheme('{{{uiThemeName}}}');
                   """
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}