// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Web;
using Microsoft.Web.WebView2.Wpf;

namespace Spectralyzer.App.Host.Controllers;

public sealed class MonacoEditorController
{
    private const string EditorContainerSelector = "#root";
    private const string EditorObject = "MonacoEditor";
    private readonly WebView2 _webView2;

    public MonacoEditorController(WebView2 webView2)
    {
        _webView2 = webView2 ?? throw new ArgumentNullException(nameof(webView2));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        const string uiThemeName = "_default";

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

    public void NavigateTo(Uri uri)
    {
        _webView2.Source = uri;
    }

    public async Task SetContentAsync(string contents)
    {
        var escapedContents = HttpUtility.JavaScriptStringEncode(contents);
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
}