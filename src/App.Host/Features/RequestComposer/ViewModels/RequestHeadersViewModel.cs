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
            new RequestHeaderViewModel("Accept", "*/*", true)
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
        var itemsToDelete = Items.Where(item => !item.IsReadOnly).ToList();

        foreach (var item in itemsToDelete)
        {
            Items.Remove(item);
        }
    }
}