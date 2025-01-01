// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestParametersViewModel : RequestItemViewModel
{
    public override string Title => "Query parameters";
    public ICommand AddCommand { get; }
    public ICommand DeleteAllCommand { get; }
    public ObservableCollection<RequestParameterViewModel> Items { get; }

    public RequestParametersViewModel()
    {
        Items = [];

        AddCommand = new RelayCommand(Add);
        DeleteAllCommand = new RelayCommand(DeleteAll);
    }

    private void Add()
    {
        Items.Add(new RequestParameterViewModel());
    }

    private void DeleteAll()
    {
        Items.Clear();
    }
}