using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Messenger.Tools;


public static class Framing
{
    private const int MaxMessage = 4 * 1024 * 1024; // 4 MB

    public static async Task WriteWithLengthAsync(Stream s, string text, CancellationToken ct = default)
    {
        byte[] payload = Encoding.UTF8.GetBytes(text);
        Span<byte> len = stackalloc byte[4];
        BitConverter.TryWriteBytes(len, payload.Length);
        await s.WriteAsync(len, ct);
        await s.WriteAsync(payload, ct);
    }

    public static async Task<string?> ReadWithLengthAsync(Stream s, CancellationToken ct = default)
    {
        byte[] lenBuf = ArrayPool<byte>.Shared.Rent(4);
        try
        {
            if (!await ReadExactAsync(s, lenBuf.AsMemory(0, 4), ct))
                return null;

            int len = BitConverter.ToInt32(lenBuf, 0);
            if (len < 0 || len > MaxMessage) throw new IOException("Invalid length.");

            byte[] buf = ArrayPool<byte>.Shared.Rent(len);
            try
            {
                if (!await ReadExactAsync(s, buf.AsMemory(0, len), ct))
                    return null;
                return Encoding.UTF8.GetString(buf, 0, len);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(lenBuf);
        }
    }

    private static async Task<bool> ReadExactAsync(Stream s, Memory<byte> buf, CancellationToken ct)
    {
        int read = 0;
        while (read < buf.Length)
        {
            int n = await s.ReadAsync(buf.Slice(read), ct);
            if (n == 0) return false;
            read += n;
        }
        return true;
    }
}


