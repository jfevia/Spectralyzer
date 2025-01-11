// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Options;

namespace Spectralyzer.Shared.Core.Windows.FileSystem;

internal sealed class FileSystemClient : IFileSystemClient
{
    private readonly IFileSystem _fileSystem;
    private readonly IOptions<ApplicationOptions> _options;

    public FileSystemClient(IOptions<ApplicationOptions> options, IFileSystem fileSystem)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public string Combine(IDirectoryInfo directoryInfo, string path)
    {
        return _fileSystem.Path.Combine(directoryInfo.FullName, path);
    }

    public FileSystemStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess)
    {
        return _fileSystem.FileStream.New(filePath, fileMode, fileAccess);
    }

    public void DeleteFile(string filePath)
    {
        _fileSystem.File.Delete(filePath);
    }

    public string GetExtension(string path)
    {
        return _fileSystem.Path.GetExtension(path);
    }

    public IDirectoryInfo GetTempPath()
    {
        var tempPath = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), _options.Value.ManufacturerName, _options.Value.ProductName);
        return _fileSystem.DirectoryInfo.New(tempPath);
    }
}