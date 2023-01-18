using DalSoft.RestClient;
using Newtonsoft.Json.Linq;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V3
    {
        public static readonly string URL_Users = Urls.GetUrl("v3/users/");

        /// <summary>
        /// Get User stuff from Ubi
        /// </summary>
        /// <param name="token">Ubi Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <returns>UsersMe or Null</returns>
        public static UsersMe? GetUsersMe(string token, string sessionId)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient($"{URL_Users}me", headers);
            var posted = client.Get<UsersMe>();
            posted.Wait();

            if (posted.Result.Status == null)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get User stuff from Ubi by UserId
        /// </summary>
        /// <param name="token">Ubi Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <param name="UserId">User Id</param>
        /// <param name="fields">Fields Filter</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetUsersMeById(string token, string sessionId, string UserId, List<string> fields)
        {
            string URL = $"{URL_Users}{UserId}";


            if (fields == null || fields.Count == 0)
            {
                //Just normal request should do anything
            }
            else
            {
                var filedscommas = String.Join(",", fields);
                URL += "?fields=" + filedscommas;
            }

            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", AppID);
            headers.Add("Content-Type", "application/json");
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }
    }
}
