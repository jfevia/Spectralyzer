// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.IO;
using System.IO.Abstractions;

namespace Spectralyzer.Shared.Core.Windows.FileSystem;

public interface IFileSystemClient
{
    string Combine(IDirectoryInfo directoryInfo, string path);
    FileSystemStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess);
    void DeleteFile(string filePath);
    string GetExtension(string path);
    IDirectoryInfo GetTempPath();
}