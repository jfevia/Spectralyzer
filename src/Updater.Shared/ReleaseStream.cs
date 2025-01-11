// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;

namespace Spectralyzer.Updater.Shared;

public sealed class ReleaseStream : Stream
{
    private readonly Stream _underlyingStream;

    public override bool CanRead => _underlyingStream.CanRead;
    public override bool CanSeek => _underlyingStream.CanSeek;
    public override bool CanWrite => _underlyingStream.CanWrite;
    public override long Length { get; }

    public override long Position
    {
        get => _underlyingStream.Position;
        set => _underlyingStream.Position = value;
    }

    public ReleaseStream(Stream stream, long length)
    {
        _underlyingStream = stream ?? throw new ArgumentNullException(nameof(stream));
        Length = length;
    }

    public override void Flush()
    {
        _underlyingStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _underlyingStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _underlyingStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _underlyingStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _underlyingStream.Write(buffer, offset, count);
    }
}