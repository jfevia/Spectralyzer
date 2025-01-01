// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestHeadersViewModel : RequestItemViewModel
{
    public override string Title => "Headers";
    public ICommand AddCommand { get; }
    public ICommand DeleteAllCommand { get; }
    public ObservableCollection<RequestHeaderViewModel> Items { get; }

    public RequestHeadersViewModel()
    {
        Items =
        [
            new RequestHeaderViewModel("Content-Type", "application/json"),
            new RequestHeaderViewModel("Accept", "application/json")
        ];

        AddCommand = new RelayCommand(Add);
        DeleteAllCommand = new RelayCommand(DeleteAll);
    }

    private void Add()
    {
        Items.Add(new RequestHeaderViewModel());
    }

    private void DeleteAll()
    {
        Items.Clear();
    }
}