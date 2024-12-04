using Google.Protobuf;
using System.Reflection.Metadata;

namespace UplayKit;

public static class Formatters
{
    public static string FormatFileSize(this ulong lsize)
    {
        double size = lsize;
        int index = 0;
        for (; size > 1024; index++)
            size /= 1024;
        return size.ToString($"0.000 {new[] { "B", "KB", "MB", "GB", "TB" }[index]}");
    }

    public static byte[] FormatUpstream(this ReadOnlySpan<byte> rawMessage)
    {
        BlobWriter blobWriter = new(4 + rawMessage.Length);
        blobWriter.WriteUInt32BE((uint)rawMessage.Length);
        blobWriter.WriteBytes(rawMessage.ToArray());
        var returner = blobWriter.ToArray();
        blobWriter.Clear();
        return returner;
    }

    public static uint FormatLength(this uint length)
    {
        BlobWriter blobWriter = new(4);
        blobWriter.WriteUInt32BE(length);
        var returner = BitConverter.ToUInt32(blobWriter.ToArray());
        blobWriter.Clear();
        return returner;
    }

    public static char FormatSliceHashChar(this string sliceId)
    {
        char[] base32 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v' };
        byte reversedValue = byte.Parse($"{sliceId[1]}{sliceId[0]}", System.Globalization.NumberStyles.HexNumber);
        int offset = (int)Math.Floor((decimal)reversedValue / 16);
        int halfOffset = reversedValue % 2 == 0 ? 0 : 16;
        return base32[offset + halfOffset];
    }

    /// <summary>
    /// Formating <paramref name="bytes"/> to any Protobuf message with a Length before the message
    /// </summary>
    /// <typeparam name="T">Any IMessage</typeparam>
    /// <param name="bytes">The Bytes</param>
    /// <returns>The type if can be parsed or Null/Default</returns>
    public static T? FormatData<T>(this ReadOnlySpan<byte> bytes) where T : IMessage<T>, new()
    {
        try
        {
            if (bytes.IsEmpty)
                return default;

            var responseLength = FormatLength(BitConverter.ToUInt32(bytes[..4]));
            if (responseLength == 0)
                return default;

            MessageParser<T> parser = new(() => new T());
            return parser.ParseFrom(bytes.Slice(4, (int)responseLength));
        }
        catch (Exception ex)
        {
            InternalEx.WriteEx(ex);
            return default;
        }
    }

    /// <summary>
    /// Formating <paramref name="bytes"/> to any Protobuf message
    /// </summary>
    /// <typeparam name="T">Any IMessage</typeparam>
    /// <param name="bytes">The Bytes</param>
    /// <returns>The type if can be parsed or Null/Default</returns>
    public static T? FormatDataNoLength<T>(this ReadOnlySpan<byte> bytes) where T : IMessage<T>, new()
    {
        try
        {
            if (bytes.IsEmpty)
                return default;

            MessageParser<T> parser = new(() => new T());
            return parser.ParseFrom(bytes);
        }
        catch (Exception ex)
        {
            InternalEx.WriteEx(ex);
            return default;
        }
    }
}
