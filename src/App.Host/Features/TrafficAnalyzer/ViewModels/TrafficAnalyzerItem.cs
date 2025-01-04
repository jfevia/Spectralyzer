// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Spectralyzer.App.Host.ViewModels;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;

public sealed class TrafficAnalyzerItem : Item
{
    private readonly IWebProxyServerFactory _webProxyServerFactory;
    private readonly ConcurrentDictionary<Guid, WebSessionViewModel> _webSessionById;
    private bool _decryptSsl;
    private bool _hasSessions;
    private bool _isCapturingTraffic;
    private int _port;
    private WebSessionViewModel? _selectedWebSession;
    private WebProxyEndpoint? _webProxyEndpoint;
    private IWebProxyServer? _webProxyServer;
    public override string Title => "Traffic analyzer";

    public ICommand ClearSessionsCommand { get; }

    public bool DecryptSsl
    {
        get => _decryptSsl;
        set => SetProperty(ref _decryptSsl, value);
    }

    public ObservableCollection<Exception> Errors { get; }

    public bool HasSessions
    {
        get => _hasSessions;
        private set => SetProperty(ref _hasSessions, value);
    }

    public bool IsCapturingTraffic
    {
        get => _isCapturingTraffic;
        private set => SetProperty(ref _isCapturingTraffic, value);
    }

    public int Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public WebSessionViewModel? SelectedWebSession
    {
        get => _selectedWebSession;
        set => SetProperty(ref _selectedWebSession, value);
    }

    public ICommand StartCaptureCommand { get; }
    public ICommand StopCaptureCommand { get; }
    public ObservableCollection<WebSessionViewModel> WebSessions { get; }

    public TrafficAnalyzerItem(IWebProxyServerFactory webProxyServerFactory)
    {
        _webProxyServerFactory = webProxyServerFactory ?? throw new ArgumentNullException(nameof(webProxyServerFactory));

        _port = 8000;
        _decryptSsl = true;

        _webSessionById = new ConcurrentDictionary<Guid, WebSessionViewModel>();
        WebSessions = [];
        WebSessions.CollectionChanged += OnWebSessionsCollectionChanged;

        Errors = [];

        StartCaptureCommand = new AsyncRelayCommand(StartCaptureAsync);
        StopCaptureCommand = new AsyncRelayCommand(StopCaptureAsync);
        ClearSessionsCommand = new RelayCommand(ClearSessions);
    }

    private static WebRequestMessageViewModel CreateWebRequestMessageViewModel(WebRequestMessage webRequestMessage)
    {
        return new WebRequestMessageViewModel(webRequestMessage.Id, webRequestMessage.RequestUri, webRequestMessage.Method);
    }

    private static WebSessionViewModel CreateWebSessionViewModel(int index, WebRequestMessage webRequestMessage, Process process)
    {
        return new WebSessionViewModel(index, process, CreateWebRequestMessageViewModel(webRequestMessage));
    }

    private void ClearSessions()
    {
        _webSessionById.Clear();
        WebSessions.Clear();
    }

    private WebSessionViewModel CreateWebSession(WebRequestMessage webRequestMessage)
    {
        var process = Process.GetProcessById(webRequestMessage.ProcessId);
        return CreateWebSessionViewModel(_webSessionById.Count, webRequestMessage, process);
    }

    private IWebProxyServer GetOrCreateWebProxyServer()
    {
        return _webProxyServer ??= _webProxyServerFactory.Create();
    }

    private async Task StartCaptureAsync(CancellationToken cancellationToken)
    {
        var webProxyServer = GetOrCreateWebProxyServer();
        webProxyServer.SendingRequest += OnSendingRequest;
        webProxyServer.ResponseReceived += OnResponseReceived;
        webProxyServer.Error += OnError;
        _webProxyEndpoint = webProxyServer.AddEndpoint(IPAddress.Any, _port, _decryptSsl);
        await webProxyServer.StartAsync(cancellationToken);
        webProxyServer.SetSystemProxy(_webProxyEndpoint);
        IsCapturingTraffic = true;
    }

    private async Task StopCaptureAsync(CancellationToken cancellationToken)
    {
        if (_webProxyServer is null || _webProxyEndpoint is null)
        {
            return;
        }

        _webProxyServer.ResetSystemProxy();
        _webProxyServer.RemoveEndpoint(_webProxyEndpoint);
        _webProxyServer.SendingRequest -= OnSendingRequest;
        _webProxyServer.ResponseReceived -= OnResponseReceived;
        _webProxyServer.Error -= OnError;
        await _webProxyServer.StopAsync(cancellationToken);
        IsCapturingTraffic = false;
    }

    private void OnError(object? sender, ExceptionEventArgs e)
    {
        if (!Application.Current.Dispatcher.CheckAccess())
        {
            Application.Current.Dispatcher.Invoke(() => OnError(sender, e));
            return;
        }

        Errors.Add(e.Exception);
    }

    private void OnResponseReceived(object? sender, WebResponseEventArgs e)
    {
        if (!Application.Current.Dispatcher.CheckAccess())
        {
            Application.Current.Dispatcher.Invoke(() => OnResponseReceived(sender, e));
            return;
        }

        if (_webSessionById.TryGetValue(e.WebResponseMessage.Id, out var webSession))
        {
            webSession.ResponseMessage.ProcessMessage(e.WebResponseMessage);
        }
    }

    private void OnSendingRequest(object? sender, WebRequestEventArgs e)
    {
        if (!Application.Current.Dispatcher.CheckAccess())
        {
            Application.Current.Dispatcher.Invoke(() => OnSendingRequest(sender, e));
            return;
        }

        var webSession = CreateWebSession(e.WebRequestMessage);
        _webSessionById[webSession.RequestMessage.RequestId] = webSession;
        WebSessions.Add(webSession);
    }

    private void OnWebSessionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is ObservableCollection<WebSessionViewModel> webSessions)
        {
            HasSessions = webSessions.Count > 0;
        }
    }
}