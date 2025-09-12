using System.IO.Compression;
using System.Text;

namespace UplayKit;

public class Parsers
{
    #region Manifest
    public static Uplay.Download.Manifest ParseManifest(Stream inputStream)
    {
        inputStream.Seek(356, SeekOrigin.Begin);
        using ZLibStream decompressor = new(inputStream, CompressionMode.Decompress);
        return Uplay.Download.Manifest.Parser.ParseFrom(decompressor);
    }
    public static void ListManifestData(string FileInput)
    {
        var inputStream = File.OpenRead(FileInput);
        using (var reader = new BinaryReader(inputStream, Encoding.UTF8, false))
        {
            var compression = reader.ReadInt32();
            var compression_method = (Uplay.Download.CompressionMethod)compression;
            Console.WriteLine("Compression Method: " + compression_method);
            var sign_lenght = reader.ReadInt32();
            Console.WriteLine("Pure Sign Lenght: " + sign_lenght);
            var manifest_length = reader.ReadInt32();
            Console.WriteLine("Manifest Lenght: " + manifest_length);
            var signed = reader.ReadBytes(sign_lenght);
            var byteString = Encoding.UTF8.GetString(signed);
            Console.WriteLine("Sign Base64: \n" + byteString);
            File.WriteAllText(FileInput + ".64sign", byteString);
        }
        inputStream.Close();
    }
    public static Uplay.Download.Manifest ParseManifestFile(string FileInput)
    {
        return ParseManifest(File.OpenRead(FileInput));
    }
    #endregion
    #region ManifestMetaData
    public static Uplay.Download.ManifestMetaData ParseMetaData(Stream inputStream)
    {
        inputStream.Seek(356, SeekOrigin.Begin);
        using ZLibStream decompressor = new(inputStream, CompressionMode.Decompress);
        return Uplay.Download.ManifestMetaData.Parser.ParseFrom(decompressor);
    }
    public static Uplay.Download.ManifestMetaData ParseMetaDataFile(string FileInput)
    {
        return ParseMetaData(File.OpenRead(FileInput));
    }
    #endregion
    #region ManifestLicenses
    public static Uplay.Download.ManifestLicenses ParseManifestLicenses(Stream inputStream)
    {
        inputStream.Seek(356, SeekOrigin.Begin);
        using ZLibStream decompressor = new(inputStream, CompressionMode.Decompress);
        return Uplay.Download.ManifestLicenses.Parser.ParseFrom(decompressor);
    }
    public static Uplay.Download.ManifestLicenses ParseManifestLicensesFile(string FileInput)
    {
        return ParseManifestLicenses(File.OpenRead(FileInput));
    }
    #endregion
    #region DownloadCahce
    public static Uplay.DownloadCache.DownloadCache ParseDownloadCache(Stream inputStream)
    {
        inputStream.Seek(4, SeekOrigin.Begin);
        return Uplay.DownloadCache.DownloadCache.Parser.ParseFrom(inputStream);
    }

    public static Uplay.DownloadCache.DownloadCache ParseDownloadCacheFile(string FileInput)
    {
        return ParseDownloadCache(File.OpenRead(FileInput));
    }
    #endregion
    #region OwnershipCache
    public static Uplay.OwnershipCache.OwnershipCache ParseOwnerShip(Stream inputStream)
    {
        inputStream.Seek(262, SeekOrigin.Begin);
        return Uplay.OwnershipCache.OwnershipCache.Parser.ParseFrom(inputStream);
    }

    public static Uplay.OwnershipCache.OwnershipCache ParseOwnerShipFile(string FileInput)
    {
        return ParseOwnerShip(File.OpenRead(FileInput));
    }
    #endregion
}
