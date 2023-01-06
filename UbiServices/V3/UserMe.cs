using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
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
            var client = new RestClient(URL_Users + "me");
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JsonConvert.DeserializeObject<UsersMe>(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
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
    }
}
