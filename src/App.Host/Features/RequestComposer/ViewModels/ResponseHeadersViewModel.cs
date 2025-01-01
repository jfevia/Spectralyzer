// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class ResponseHeadersViewModel : ResponseItemViewModel
{
    public override string Title => "Headers";
    public ObservableCollection<ResponseHeaderViewModel> Items { get; }

    public ResponseHeadersViewModel()
    {
        Items =
        [
            new ResponseHeaderViewModel("Content-Type", "application/json"),
            new ResponseHeaderViewModel("Accept", "application/json")
        ];
    }
}