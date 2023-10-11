using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Org.Websn.Extensions
{
    public sealed class StreamPackage : Stream, IStreamable
    {
        private readonly Stream _buffer = new MemoryStream();

        public Stream BaseStream { get; }

        public override bool CanRead => _buffer.CanRead;

        public override bool CanSeek => _buffer.CanSeek;

        public override bool CanWrite => _buffer.CanWrite;

        public override long Length => _buffer.Length;

        public override long Position { get => _buffer.Position; set => _buffer.Position = value; }

        public override bool CanTimeout => _buffer.CanTimeout;

        public override int ReadTimeout { get => _buffer.ReadTimeout; set => _buffer.ReadTimeout = value; }

        public override int WriteTimeout { get => _buffer.WriteTimeout; set => _buffer.WriteTimeout = value; }

        public StreamPackage(Stream stream = null, Stream buffer = null)
        {
            BaseStream = stream;
#if NET5_0_OR_GREATER
            buffer ??= new MemoryStream();
#else
            if (buffer == null) buffer = new MemoryStream();
#endif
            if (!buffer.CanSeek) throw new ArgumentException("The buffer stream object must be able to seek", nameof(buffer));
            if (!buffer.CanWrite) throw new ArgumentException("The buffer stream object must be able to be written to", nameof(buffer));

            _buffer = buffer;
        }

        public override void Flush()
        {
            _buffer.Flush();
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _buffer.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _buffer.Read(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return await _buffer.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _buffer.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _buffer.BeginWrite(buffer, offset, count, callback, state);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _buffer.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _buffer.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, offset, count);
        }

#if NET5_0_OR_GREATER
        public override void CopyTo(Stream destination, int bufferSize)
        {
            _buffer.CopyTo(destination, bufferSize);
        }
#endif

        public override void Close()
        {
            _buffer.Close();
        }

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            await _buffer.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _buffer.Dispose();
        }

#if NET5_0_OR_GREATER

        public override ValueTask DisposeAsync()
        {
            return _buffer.DisposeAsync();
        }
#endif

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _buffer.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _buffer.EndWrite(asyncResult);
        }

#if NET5_0_OR_GREATER
        [Obsolete()]
#endif
        public override object InitializeLifetimeService()
        {
            return _buffer.InitializeLifetimeService();
        }

#if NET5_0_OR_GREATER
        public override int Read(Span<byte> buffer)
        {
            return _buffer.Read(buffer);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _buffer.ReadAsync(buffer, cancellationToken);
        }
#endif

        public override int ReadByte()
        {
            return _buffer.ReadByte();
        }

#if NET5_0_OR_GREATER
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _buffer.Write(buffer);
        }
#endif

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _buffer.WriteAsync(buffer, offset, count, cancellationToken);
        }

#if NET5_0_OR_GREATER
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _buffer.WriteAsync(buffer, cancellationToken);
        }
#endif

        public override void WriteByte(byte value)
        {
            _buffer.WriteByte(value);
        }


        /// <summary>
        /// Write the <see cref="StreamPackage"/> to the <see cref="BaseStream"/> or <paramref name="target"/>
        /// </summary>
        public void Submit(Stream target = null)
        {
#if NET5_0_OR_GREATER
            target ??= BaseStream;
#else
            if (target == null) target = BaseStream;
#endif

            lock (target)
            {
                long bufferPos = _buffer.Position;
                int lastRead;
                _buffer.Position = 0;
                byte[] transferBuffer = new byte[4096];
                while (_buffer.Position < bufferPos)
                {
                    lastRead = _buffer.Read(transferBuffer, 0, (int)Math.Min(bufferPos - _buffer.Position, 4096L));
                    target.Write(transferBuffer, 0, lastRead);
                }
                _buffer.Position = bufferPos;
            }
        }

        /// <summary>
        /// Clears the buffer of the <see cref="StreamPackage"/>
        /// </summary>
        public void Reset()
        {
            long bufferPos = _buffer.Position;
            _buffer.Position = 0;
            while (--bufferPos > 0) _buffer.WriteByte(0);
            _buffer.Position = 0;
        }

        public void ToStream(Stream stream)
        {
            Submit(stream);
        }

        /// <summary>
        /// Create a byte array from the internal buffer
        /// </summary>
        public byte[] ToByteArray()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                Submit(memStream);
                memStream.Position = 0;
                return memStream.ToArray();
            }
        }

        ~StreamPackage()
        {
            Dispose();
        }
    }
}
