// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Spectralyzer.Updater.Shared.GitHub;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
internal class GitHubRelease
{
    [JsonPropertyName("assets")]
    public GitHubReleaseAsset[] Assets { get; set; } = null!;

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
}