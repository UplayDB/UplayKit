using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace UbiServices.Public
{
    public partial class V3
    {
        public static readonly string URL_Devices = Urls.GetUrl("v3/users/me/trusteddevices");

        /// <summary>
        /// Get Trusted Devices
        /// </summary>
        /// <param name="token">UbiV1 Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetTrustedDevices(string token, string sessionId)
        {
            var client = new RestClient(URL_Devices);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);

            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// Update Trusted Device with an new Name
        /// </summary>
        /// <param name="token">UbiV1 Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <param name="deviceId">Device Id</param>
        /// <param name="DeviceName">New Device Name</param>
        /// <returns>JObject or Null</returns>
        public static JObject? UpdateTrustedDevices(string token, string sessionId, string deviceId, string DeviceName)
        {
            var client = new RestClient(URL_Devices + $"/{deviceId}");
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);
            var rem = new
            {
                friendlyName = DeviceName
            };
            request.AddJsonBody(rem);
            RestResponse response = client.PutAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// [WARN] It will delete Trusted Device!
        /// </summary>
        /// <param name="token">UbiV1 Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <param name="deviceId">Device Id</param>
        /// <returns>StatusCode or Null</returns>
        public static HttpStatusCode? DeleteTrustedDevices(string token, string sessionId, string deviceId)
        {
            var client = new RestClient(URL_Devices + $"/{deviceId}");
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);

            RestResponse response = client.DeleteAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return response.StatusCode;
            }
            return null;
        }

        /// <summary>
        /// Generating Device ID from Device Name
        /// </summary>
        /// <param name="DeviceName">Device Name</param>
        /// <returns></returns>
        public static string GenerateDeviceId(string DeviceName)
        {
            var data = Encoding.UTF8.GetBytes(DeviceName);
            // key
            var key = Encoding.UTF8.GetBytes("UbiServices");

            // Create HMAC-MD5 Algorithm;
            var hmac = new HMACMD5(key);

            // Compute hash.
            var hashBytes = hmac.ComputeHash(data);

            // Convert to HEX string.
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
