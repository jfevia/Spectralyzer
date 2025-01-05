// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections;
using System.Diagnostics;
using System.Windows;
using Spectralyzer.App.Host.Controllers;

namespace Spectralyzer.App.Host.Controls;

public partial class HttpViewer
{
    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(HttpViewer), new PropertyMetadata(false, OnIsReadOnlyChanged));

    public static readonly DependencyProperty BodyProperty =
        DependencyProperty.Register(nameof(Body), typeof(string), typeof(HttpViewer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBodyChanged));

    public static readonly DependencyProperty FormatItemsSourceProperty =
        DependencyProperty.Register(nameof(FormatItemsSource), typeof(IEnumerable<string>), typeof(HttpViewer), new PropertyMetadata(null));


    public static readonly DependencyProperty SelectedFormatProperty =
        DependencyProperty.Register(nameof(SelectedFormat), typeof(string), typeof(HttpViewer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedFormatChanged));

    private MonacoEditorController? _monacoEditorController;

    public string? Body
    {
        get => (string?)GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    public IEnumerable FormatItemsSource
    {
        get => (IEnumerable)GetValue(FormatItemsSourceProperty);
        set => SetValue(FormatItemsSourceProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public string? SelectedFormat
    {
        get => (string?)GetValue(SelectedFormatProperty);
        set => SetValue(SelectedFormatProperty, value);
    }

    public HttpViewer()
    {
        Initialized += OnInitialized;
        InitializeComponent();
    }

    private static void OnBodyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((HttpViewer)d).OnBodyChanged((string?)e.NewValue);
    }

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((HttpViewer)d).OnIsReadOnlyChanged((bool)e.NewValue);
    }

    private static void OnSelectedFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((HttpViewer)d).OnSelectedFormatChanged((string?)e.NewValue);
    }

    private async void OnBodyChanged(string? newValue)
    {
        try
        {
            if (_monacoEditorController is not null)
            {
                await _monacoEditorController.SetContentAsync(newValue);
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }

    private async void OnIsReadOnlyChanged(bool newValue)
    {
        try
        {
            if (_monacoEditorController is null)
            {
                return;
            }

            await _monacoEditorController.SetIsReadOnlyAsync(newValue);
        }
        catch (Exception)
        {
            // Ignored
        }
    }

    private async void OnSelectedFormatChanged(string? value)
    {
        try
        {
            if (_monacoEditorController is not null && value is not null)
            {
                await _monacoEditorController.SetLanguageAsync(value);

                if (IsReadOnly)
                {
                    await _monacoEditorController.SetIsReadOnlyAsync(false);
                }

                await _monacoEditorController.FormatDocumentAsync();

                if (IsReadOnly)
                {
                    await _monacoEditorController.SetIsReadOnlyAsync(true);
                }
            }
        }
        catch (Exception)
        {
            // Ignored
        }
    }

    private async void OnInitialized(object? sender, EventArgs e)
    {
        try
        {
            Initialized -= OnInitialized;

            _monacoEditorController = new MonacoEditorController(WebView2);
            await _monacoEditorController.InitializeAsync();
            OnSelectedFormatChanged(SelectedFormat);
            OnBodyChanged(Body);
            OnIsReadOnlyChanged(IsReadOnly);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}