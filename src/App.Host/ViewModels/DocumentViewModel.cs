// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Spectralyzer.App.Host.ViewModels;

public sealed class DocumentViewModel : ObservableObject
{
    public IHighlightingDefinition? HighlightingDefinition { get; }
    public TextDocument Root { get; }

    public DocumentViewModel(string? highlightDefinitionName, string? bodyAsString)
    {
        HighlightingDefinition = highlightDefinitionName is not null ? HighlightingManager.Instance.GetDefinition(highlightDefinitionName) : null;
        Root = bodyAsString is not null ? new TextDocument(bodyAsString) : new TextDocument();
    }
}