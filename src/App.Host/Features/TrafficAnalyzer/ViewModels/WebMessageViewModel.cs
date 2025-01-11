// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Spectralyzer.Core;
using Spectralyzer.Shared.UI.ComponentModel;

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

    public Guid RequestId { get; }

    public string? SelectedFormat
    {
        get => _selectedFormat;
        set => SetProperty(ref _selectedFormat, value);
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

    private void OnBodyChanged(string? value)
    {
        HttpView = GenerateHttpView();
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