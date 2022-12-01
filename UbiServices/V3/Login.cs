using Newtonsoft.Json;
using RestSharp;
using System.Text;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V3
    {
        public static readonly string AppID = "f68a4bb5-608a-4ff2-8123-be8ef797e0a6";
        public static string UserAgent = "Massgate";
        public static readonly string URL = "https://public-ubiservices.ubi.com/v3/users/me";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static LoginJson? GetLogin(string email, string password)
        {
            string b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
            return GetLoginBase64(b64);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b64"></param>
        /// <returns></returns>
        public static LoginJson? GetLoginBase64(string b64)
        {
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Authorization", $"Basic {b64}");
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Ubi-RequestedPlatformType", "uplay");
            request.AddHeader("Ubi-AppId", AppID);
            var rem = new
            {
                rememberMe = true
            };
            request.AddJsonBody(rem);

            RestResponse response = client.PostAsync(request).Result;

            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<LoginJson>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tfaTicket"></param>
        /// <param name="tfaCode"></param>
        /// <returns></returns>
        public static LoginJson? GetLogin2FA(string tfaTicket, string tfaCode)
        {
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Authorization", $"ubi_2fa_v1 t={tfaTicket}");
            request.AddHeader("Ubi-2FACode", tfaCode);
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Ubi-RequestedPlatformType", "uplay");
            request.AddHeader("Ubi-AppId", AppID);
            var rem = new
            {
                rememberMe = true
            };
            request.AddJsonBody(rem);

            RestResponse response = client.PostAsync(request).Result;

            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<LoginJson>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rememberTicket"></param>
        /// <returns></returns>
        public static LoginJson? GetLoginRemember(string rememberTicket)
        {
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Authorization", $"rm_v1 t={rememberTicket}");
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Ubi-RequestedPlatformType", "uplay");
            request.AddHeader("Ubi-AppId", AppID);
            var rem = new
            {
                rememberMe = true
            };
            request.AddJsonBody(rem);

            RestResponse response = client.PostAsync(request).Result;

            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<LoginJson>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rememberTicket"></param>
        /// <param name="rememberDeviceTicket"></param>
        /// <returns></returns>
        public static LoginJson? GetLoginRememberDevice(string rememberTicket, string rememberDeviceTicket)
        {
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Authorization", $"rm_v1 t={rememberTicket}");
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Ubi-RequestedPlatformType", "uplay");
            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Ubi-RememberDeviceTicket", rememberDeviceTicket);
            var rem = new
            {
                rememberMe = true
            };
            request.AddJsonBody(rem);

            RestResponse response = client.PostAsync(request).Result;

            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<LoginJson>(response.Content);
            }

            return null;
        }
    }
}