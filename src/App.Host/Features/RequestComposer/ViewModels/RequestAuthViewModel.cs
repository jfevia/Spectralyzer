// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace Spectralyzer.App.Host.Features.RequestComposer.ViewModels;

public sealed class RequestAuthViewModel : RequestItemViewModel
{
    private string? _selectedTokenType;

    public override string Title => "Authentication";

    public string? SelectedTokenType
    {
        get => _selectedTokenType;
        set => SetProperty(ref _selectedTokenType, value);
    }

    public IEnumerable<string> TokenTypes { get; }

    public RequestAuthViewModel()
    {
        TokenTypes =
        [
            "None",
            "API key",
            "Basic",
            "Digest",
            "NTLM",
            "OAuth 1.0",
            "OAuth 2.0",
            "Bearer token"
        ];

        SelectedTokenType = TokenTypes.FirstOrDefault();
    }
}