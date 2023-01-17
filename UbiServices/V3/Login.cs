using DalSoft.RestClient;
using System.Text;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V3
    {
        public static string AppID = "f68a4bb5-608a-4ff2-8123-be8ef797e0a6";
        public static string UserAgent = "Massgate";
        public static readonly string URL_Session = Urls.GetUrl("v3/profiles/sessions");
        static RemMe rem = new RemMe { RememberMe = true };

        internal static LoginJson? DoLogin(Dictionary<string, string> headers, RemMe remMe)
        {
            var client = new RestClient(URL_Session, headers);
            var posted = client.Post<RemMe, LoginJson>(remMe);
            posted.Wait();

            if (posted.Result.SessionId == "")
            {
                return null;
            }

            return posted.Result;

        }
        /// <summary>
        /// Login via Email and Password
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <returns>LoginJson or Null</returns>
        public static LoginJson? Login(string email, string password)
        {
            if (!Validations.EmailValidation(email))
                return null;
            string b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
            return LoginBase64(b64);
        }

        /// <summary>
        /// Login via Base64 mail and password
        /// </summary>
        /// <param name="b64">Base64 of Email:Password</param>
        /// <returns>LoginJson or Null</returns>
        public static LoginJson? LoginBase64(string b64)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Basic {b64}");
            headers.Add("User-Agent", UserAgent);
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");
            
            return DoLogin(headers,rem);
        }

        /// <summary>
        /// Login via 2FA ticket and Code
        /// </summary>
        /// <param name="tfaTicket">2FA Ticket</param>
        /// <param name="tfaCode">Code from Auth device (mail,google)</param>
        /// <returns>LoginJson or Null</returns>
        public static LoginJson? Login2FA(string tfaTicket, string tfaCode)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"ubi_2fa_v1 t={tfaTicket}");
            headers.Add("User-Agent", UserAgent); 
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-2FACode", tfaCode);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");

            return DoLogin(headers, rem);

        }


        /// <summary>
        /// Login via 2FA ticket and Code Plus Remember Device stuff
        /// </summary>
        /// <param name="tfaTicket">2FA Ticket</param>
        /// <param name="tfaCode">Code from Auth device (mail,google)</param>
        /// <param name="trustedId">Device Id</param>
        /// <param name="TrustedName">Device Name</param>
        /// <returns></returns>
        public static LoginJson? Login2FA_Device(string tfaTicket, string tfaCode, string trustedId, string TrustedName)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"ubi_2fa_v1 t={tfaTicket}");
            headers.Add("Ubi-2FACode", tfaCode);
            headers.Add("User-Agent", UserAgent);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");
            headers.Add("Ubi-AppId", AppID);

            var rem = new RemMe() { RememberMe = true, TrustedDevice = new() { FriendlyName = TrustedName, Id = trustedId } };

            return DoLogin(headers, rem);
        }

        /// <summary>
        /// Login via Remember Ticket
        /// </summary>
        /// <param name="rememberTicket">Remember Ticket</param>
        /// <returns>LoginJson or Null</returns>
        public static LoginJson? LoginRemember(string rememberTicket)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"rm_v1 t={rememberTicket}");
            headers.Add("User-Agent", UserAgent);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");
            headers.Add("Ubi-AppId", AppID);

            return DoLogin(headers, rem);
        }

        /// <summary>
        /// ReNewing current Token
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <param name="session">Session Id</param>
        /// <returns></returns>
        public static LoginJson? LoginRenew(string ticket, string session)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={ticket}");
            headers.Add("User-Agent", UserAgent);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-SessionId", session);

            return DoLogin(headers, rem);
        }

        /// <summary>
        /// Login via Rem Ticket and Device ticket
        /// </summary>
        /// <param name="rememberTicket">Remember Ticket</param>
        /// <param name="rememberDeviceTicket">Device Ticket</param>
        /// <returns>LoginJson or Null</returns>
        public static LoginJson? LoginRememberDevice(string rememberTicket, string rememberDeviceTicket)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"rm_v1 t={rememberTicket}");
            headers.Add("User-Agent", UserAgent);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-RequestedPlatformType", "uplay");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-RememberDeviceTicket", rememberDeviceTicket);

            return DoLogin(headers, rem);
        }
    }
}