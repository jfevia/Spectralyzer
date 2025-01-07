// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Spectralyzer.Updater.Core.GitHub;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
internal class GitHubRelease
{
    [JsonPropertyName("assets")]
    public GitHubAsset[] Assets { get; set; } = null!;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
}