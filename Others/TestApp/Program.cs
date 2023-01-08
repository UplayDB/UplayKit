﻿using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;
using UbiServices.Records;
using UplayKit;
using UplayKit.Connection;
using static UbiServices.Public.V3;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(String.Join(",", args));
            Console.ReadLine();
            LoginJson? login;
            if (HasParameter(args, "-b64"))
            {
                var b64 = GetParameter(args, "-b64", "");
                Console.WriteLine(b64);
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
            //var login = UbiServices.Public.V3.LoginBase64("");
            if (login != null)
            {
                File.WriteAllText("login.txt", JsonConvert.SerializeObject(login));
                /*
                login = LoginRememberDevice(login.RememberMeTicket, "ewogICJ0eXAiOiAiSldFIiwKICAiZW5jIjogIkExMjhDQkMiLAogICJpdiI6ICJvTE9XVjdEZGE0UzBMdEV4U3dvU09RIiwKICAiemlwIjogIkRFRiIsCiAgImludCI6ICJIUzI1NiIsCiAgImtpZCI6ICIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiCn0.gQyjnyrYCxHEl21zjvMOJtOwPjp70JHXqCIblGETQlTWblPF0AtC3EKS4zEJu6fsXcUXV0-6ogkMc8pYsdSrqCn3wei4huShJvzaZ7d49Iz9dPRV9n9IoE9aud0BGQIdaIFogDKxlyyW3vwS7LxFOXRaGtfrYNTtbj7Z8S_0hxuVO0LLOAkr9CBBGjFSlax87T64s9piqhG058yeG28-RHUEeAiB4wWpBigByupsm0dhOViPL_6o4Jg9efng6QxHXj8A4H1s0I-REFgwzBGl8Av2peSvKJ2PStEaGHAXAWU8zGDZG6STewuIPtAHBTFp.m_hZGY8H94Diq9fwJ6TsZ3KSNoRX4Y67bh0Efs4gfUw");
                File.WriteAllText("login_rem.txt", JsonConvert.SerializeObject(login));
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
                File.WriteAllText("login_2fa.txt", JsonConvert.SerializeObject(login));
                */
                Debug.isDebug = true;
                DemuxSocket socket = new();
                socket.StopTheCheck = true;
                socket.VersionCheck();

                socket.PushVersion();

                socket.Authenticate(login.Ticket);
                OwnershipConnection ownership = new(socket);
                var games = ownership.GetOwnedGames(true);
                ownership.PushEvent += Ownership_PushEvent;
                StoreConnection storeConnection = new(socket);

                storeConnection.Init();
                storeConnection.GetStore();


                Console.WriteLine("end?");
                Console.ReadLine();
                Console.WriteLine();
                socket.Close();
            }

            /*
            List<UbiServices.Records.Request> requests = new();
            requests.Add(
                new()
                {   IndexName = "ie_product_suggestion",
                    Params = "hitsPerPage=100&page=0&highlightPreTag=__ais-highlight__&highlightPostTag=__%2Fais-highlight__&facets=%5B%5D&tagFilters=&analytics=false"
                }
            );

            


            var result = UbiServices.Store.AlgoliaSearch.PostStoreAlgoliaSearch(requests);

            if (result != null)
            {
                JArray resultArray = (JArray)result["results"];

                for (int i = 0; i < resultArray.Count; i++)
                {
                    JObject resultsAObject = JObject.FromObject(resultArray[i]);
                    JArray resultsAHits = (JArray)resultsAObject["hits"];
                    //Console.WriteLine(o2);
                    for (int i2 = 0; i2 < resultsAHits.Count; i2++)
                    {
                        JObject finalJson = JObject.FromObject(resultsAHits[i2]);
                        JValue title = (JValue)finalJson["title"];
                        JValue id = (JValue)finalJson["id"];
                        Console.WriteLine($"{title} ({id})");
                    }
                }
            }
            */

        }



        private static void Ownership_PushEvent(object? sender, Uplay.Ownership.Push e)
        {
            Console.WriteLine(e.ToString());
            File.AppendAllText("ownership_push_event.txt", e.ToString());
        }
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
    }
}
