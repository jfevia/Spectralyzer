// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.IO;

namespace Spectralyzer.Updater.Core;

public sealed class ReleaseStream : Stream
{
    private readonly Stream _stream;
    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length { get; }

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    internal ReleaseStream(long length, Stream stream)
    {
        Length = length;
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }
}