// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class CookiesViewModel : ResponseItemViewModel
{
    public override string Title => "Cookies";
    public ICommand AddCommand { get; }
    public ICommand DeleteAllCommand { get; }
    public ObservableCollection<CookieViewModel> Items { get; }

    public CookiesViewModel()
    {
        Items = [];

        AddCommand = new RelayCommand(Add);
        DeleteAllCommand = new RelayCommand(DeleteAll);
    }

    private void Add()
    {
        Items.Add(new CookieViewModel());
    }

    private void DeleteAll()
    {
        Items.Clear();
    }
}