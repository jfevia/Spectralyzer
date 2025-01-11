// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

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
    private bool _isReadOnly;
    private string? _selectedFormat;

    public override string Title => "Body";

    public string? Body
    {
        get => _body;
        set => SetProperty(ref _body, value);
    }

    public IEnumerable<string> Formats { get; }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        protected set => SetProperty(ref _isReadOnly, value);
    }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value);
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
    }
}