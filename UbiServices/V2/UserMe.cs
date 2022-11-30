using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices
{
    public partial class V2
    {
        public static v2UserMe? GetUsersMe(string token, string sessionId)
        {
            string URL = $"https://public-ubiservices.ubi.com/v2/users/me";
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<v2UserMe>(response.Content);
            }
            return null;
        }
    }
}
