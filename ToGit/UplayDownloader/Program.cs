﻿using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using UbiServices.Records;
using UplayKit;
using UplayKit.Connection;
using static UbiServices.Public.V3;

namespace UplayDownloader
{
    public class Program
    {
        public static string OWToken = "";
        static void Main(string[] args)
        {
            bool haslocal = HasParameter(args, "-local");
            UbiServices.Urls.IsLocalTest = haslocal;
            #region Login
            LoginJson? login;
            if (HasParameter(args, "-b64"))
            {
                var b64 = GetParameter(args, "-b64", "");
                login = LoginBase64(b64);
            }
            else if ((HasParameter(args, "-username") || HasParameter(args, "-user")) && (HasParameter(args, "-password") || HasParameter(args, "-pass")))
            {
                var username = GetParameter<string>(args, "-username") ?? GetParameter<string>(args, "-user");
                var password = GetParameter<string>(args, "-password") ?? GetParameter<string>(args, "-pass");
                login = Login(username, password);
            }
            else
            {
                Console.WriteLine("Please enter your Email:");
                string username = Console.ReadLine()!;
                Console.WriteLine("Please enter your Password:");
                string password = ReadPassword();
                login = Login(username, password);
            }
            if (login.Ticket == null)
            {
                Console.WriteLine("Your account has 2FA, please enter your code:");
                var code2fa = Console.ReadLine();
                if (code2fa == null)
                {
                    Console.WriteLine("Code cannot be empty!");
                    Environment.Exit(1);
                }
                login = Login2FA(login.TwoFactorAuthenticationTicket, code2fa);
            }
            // Last login check
            if (login == null)
            {
                Console.WriteLine("Login failed");
                Environment.Exit(1);
            }
            #endregion
            #region Starting Connections, Getting game
            Debug.isDebug = HasParameter(args, "-debug");
            Console.WriteLine(login.Ticket);
            DemuxSocket socket = new(haslocal);
            socket.StopTheCheck = true;
            Console.WriteLine("Yeet");
            Console.WriteLine(socket.VersionCheck());
            socket.PushVersion();
            Console.WriteLine(socket.Authenticate(login.Ticket));
            OwnershipConnection ownershipConnection = new(socket);
            DownloadConnection downloadConnection = new(socket);
            var owned = ownershipConnection.GetOwnedGames(false);
            if (owned == null)
                Environment.Exit(1);
            #endregion
            #region Printing games

            var productId = GetParameter<uint>(args, "-product", 0);
            var manifest = GetParameter(args, "-manifest", "");
            var manifest_path = GetParameter(args, "-manifestpath", "");
            var product_manifest = productId + "_" + manifest;
            Uplay.Download.Manifest parsedManifest = new();
            RestClient rc = new();

            if (productId == 0 && manifest == "")
            {
                owned = owned.Where(game => game.LatestManifest.Trim().Length > 0).ToList();
                owned = owned.Where(game => game.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game).ToList();


                Console.WriteLine("-1) Your games:.");
                Console.WriteLine("----------------------");
                int gameIds = 0;
                foreach (var game in owned)
                {
                    Console.WriteLine($"\n({gameIds}) ProductId ({game.ProductId}) Manifest {game.LatestManifest}");
                    gameIds++;
                }
                Console.WriteLine("Please select:");
                Console.ReadLine();

                int selection = int.Parse(Console.ReadLine()!);
                bool manifestfile = false;
                if (selection == -1)
                {
                    Console.WriteLine("> Input the 20-byte long manifest identifier:");
                    manifest = Console.ReadLine()!.Trim();

                    if (manifest.Contains(".manifest")) { manifestfile = true; }

                    Console.WriteLine("> Input the productId:");
                    productId = uint.Parse(Console.ReadLine()!.Trim());
                }
                else if (selection <= gameIds)
                {
                    manifest = owned[selection].LatestManifest;
                    productId = owned[selection].ProductId;
                }



                if (manifestfile)
                {
                    parsedManifest = Parsers.ParseManifestFile(manifest);
                    string ownershipToken = ownershipConnection.GetOwnershipToken(productId);
                    if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                    OWToken = ownershipToken;
                    downloadConnection.InitDownloadToken(ownershipToken);
                }
                else
                {
                    string ownershipToken = ownershipConnection.GetOwnershipToken(productId);
                    if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                    OWToken = ownershipToken;
                    downloadConnection.InitDownloadToken(ownershipToken);
                    string manifestUrl = downloadConnection.GetUrl(manifest, productId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(product_manifest + ".manifest", manifestBytes);
                    parsedManifest = Parsers.ParseManifestFile(product_manifest + ".manifest");
                }

            }
            else
            {
                string ownershipToken = ownershipConnection.GetOwnershipToken(productId);
                if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                OWToken = ownershipToken;
                downloadConnection.InitDownloadToken(ownershipToken);

                if (manifest_path != "")
                {
                    parsedManifest = Parsers.ParseManifestFile(manifest_path);
                }
                else
                {
                    string manifestUrl = downloadConnection.GetUrl(manifest, productId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(product_manifest + ".manifest", manifestBytes);
                    parsedManifest = Parsers.ParseManifestFile(product_manifest + ".manifest");
                }
            }

            if (HasParameter(args, "-addons"))
            {
                string LicenseURL = downloadConnection.GetUrl(manifest, productId, "license");
                var License = rc.DownloadData(new(LicenseURL));
                if (License == null)
                    throw new("License not found?");
                File.WriteAllBytes(product_manifest + ".license", License);

                string MetadataURL = downloadConnection.GetUrl(manifest, productId, "metadata");
                var Metadata = rc.DownloadData(new(MetadataURL));
                if (Metadata == null)
                    throw new("Metadata not found?");
                File.WriteAllBytes(product_manifest + ".metadata", Metadata);
            }
            rc.Dispose();
            #endregion
            #region Compression Print
            Console.WriteLine($"\nDownloaded and parsed manifest successfully:");
            Console.WriteLine($"Compression Method: {parsedManifest.CompressionMethod} IsCompressed? {parsedManifest.IsCompressed} Version {parsedManifest.Version}");
            if (parsedManifest.CompressionMethod == Uplay.Download.CompressionMethod.Lzham) { Console.WriteLine("LZHAM ISNT SUPPORTED!"); }
            #endregion
            #region Lang Chunks
            List<Uplay.Download.File> files = new();
            var lang = GetParameter(args, "-lang", "default");

            if (lang == "default")
            {
                Console.WriteLine("Languages to use (just press enter to choose nothing, and all for all chunks)");
                parsedManifest.Languages.ToList().ForEach(x => Console.WriteLine(x.Code));

                var langchoosed = Console.ReadLine();

                if (!string.IsNullOrEmpty(langchoosed))
                {
                    if (langchoosed == "all")
                    {
                        files = ChunkManager.AllFiles(parsedManifest);
                    }
                    files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));
                    lang = langchoosed;
                    files.AddRange(ChunkManager.AddLanguage(parsedManifest, lang));
                }
                else
                {
                    files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));

                }
            }
            else
            {
                files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));
                files.AddRange(ChunkManager.AddLanguage(parsedManifest, lang));
            }
            #endregion
            #region Skipping files from chunk
            List<string> skip_files = new();

            if (HasParameter(args, "-skip"))
            {
                var skipping = GetParameter(args, "-skip", "skip.txt");
                if (File.Exists(skipping))
                {
                    var lines = File.ReadAllLines(skipping);
                    skip_files.AddRange(lines);
                    Console.WriteLine("Skipping files added");
                }
                files = ChunkManager.RemoveSkipFiles(files, skip_files);
            }
            #endregion
            #region Get Path and Create
            Console.WriteLine("\tFiles Ready to work\n");
            string downloadPath = GetParameter(args, "-dir", $"{Directory.GetCurrentDirectory()}\\{productId}\\{manifest}\\");
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            #endregion
            #region Saving
            Saving.Root saving = new();
            var savingpath = Path.Combine(downloadPath, ".UD\\saved.bin");
            Directory.CreateDirectory(Path.GetDirectoryName(savingpath));
            if (File.Exists(savingpath))
            {
                var readedBin = Saving.Read(savingpath);
                if (readedBin == null)
                {
                    saving = Saving.MakeNew(productId, manifest, parsedManifest);
                }
                else
                {
                    saving = readedBin;
                }
            }
            else
            {
                saving = Saving.MakeNew(productId, manifest, parsedManifest);
            }
            if (HasParameter(args, "-filetosaved"))
            {
                File.WriteAllText(savingpath + ".json", JsonConvert.SerializeObject(saving));
            }
            #endregion
            #region Verify + Downloading
            if (HasParameter(args, "-verify"))
            {
                files = Verifier.Verify(files, saving, downloadPath);
            }
            Downloader.DownloadWorker(files, downloadPath, downloadConnection, productId, saving);
            #endregion
            #region Closing and GoodBye
            Console.WriteLine("Goodbye!");
            Console.ReadLine();
            downloadConnection.Close();
            ownershipConnection.Close();
            socket.Close();
            #endregion
        }
        #region Other Functions
        static int IndexOfParam(string[] args, string param)
        {
            for (var x = 0; x < args.Length; ++x)
            {
                if (args[x].Equals(param, StringComparison.OrdinalIgnoreCase))
                    return x;
            }

            return -1;
        }

        static bool HasParameter(string[] args, string param)
        {
            return IndexOfParam(args, param) > -1;
        }

        static T GetParameter<T>(string[] args, string param, T defaultValue = default(T))
        {
            var index = IndexOfParam(args, param);

            if (index == -1 || index == (args.Length - 1))
                return defaultValue;

            var strParam = args[index + 1];

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(strParam);
            }

            return default(T);
        }

        //Validate the eamil address for you
        public static bool EmailValidation(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Literally stolen from SteamRE guys.
        public static string ReadPassword()
        {
            ConsoleKeyInfo keyInfo;
            var password = new StringBuilder();

            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }

                    continue;
                }
                /* Printable ASCII characters only */
                var c = keyInfo.KeyChar;
                if (c >= ' ' && c <= '~')
                {
                    password.Append(c);
                    Console.Write('*');
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            return password.ToString();
        }
        #endregion
    }
}