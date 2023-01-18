using DalSoft.RestClient;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UbiServices.Records;

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
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient(URL_Devices, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
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
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient($"{URL_Devices}/{deviceId}", headers);
            var trustedDevice = new RemMe.trustedDevice
            {
                FriendlyName = DeviceName
            };

            var posted = client.Put<RemMe.trustedDevice, JObject>(trustedDevice);
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
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
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient($"{URL_Devices}/{deviceId}", headers);
            var posted = client.Delete<HttpResponseMessage>(); //should work
            posted.Wait();
            if (posted.Result.IsSuccessStatusCode == false)
                return null;

            return posted.Result.StatusCode;
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
