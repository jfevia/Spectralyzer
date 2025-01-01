// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class ResponsePreviewViewModel : ResponseItemViewModel
{
    private string? _body;
    private string? _selectedFormat;

    public override string Title => "Preview";

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value);
    }

    public IEnumerable<string> Formats { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value);
    }

    public ResponsePreviewViewModel()
    {
        Formats =
        [
            "JSON",
            "XML",
            "None"
        ];
        
        SelectedFormat = Formats.FirstOrDefault();
    }
}