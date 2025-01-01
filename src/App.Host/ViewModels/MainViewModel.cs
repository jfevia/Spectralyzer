// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using Spectralyzer.App.Host.Features.RequestComposer;
using Spectralyzer.App.Host.Features.RequestComposer.ViewModels;
using Spectralyzer.App.Host.Features.TrafficAnalyzer;
using Spectralyzer.App.Host.Features.TrafficAnalyzer.ViewModels;
using Spectralyzer.Core;

namespace Spectralyzer.App.Host.ViewModels;

public class MainViewModel : ObservableObject
{
    private Item? _selectedItem;

    public IEnumerable<Item> Items { get; }

    public Item? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public MainViewModel(IWebProxyServerFactory webProxyServerFactory)
    {
        Items = new List<Item>
        {
            new RequestComposerItem(),
            new TrafficAnalyzerItem(webProxyServerFactory)
        };
        SelectedItem = Items.FirstOrDefault();
    }
}