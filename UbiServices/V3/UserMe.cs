using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices
{
    public partial class V3
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static UsersMe? GetUsersMe(string token, string sessionId)
        {
            string URL = $"https://public-ubiservices.ubi.com/v3/users/me";
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<UsersMe>(response.Content);
            }
            return null;
        }
    }
}
