using Newtonsoft.Json;
using RestSharp;

namespace UbiServices
{
    partial class V1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sessionId"></param>
        /// <param name="defaultOnly"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static CatalogRoot? GetCatalog(string token, string sessionId, bool defaultOnly = true, uint offset = 0, uint limit = 100)
        {
            string URL = $"https://public-ubiservices.ubi.com/v1/spaces/global/ubiconnect/games/api/catalog";

            URL += "&defaultOnly=" + defaultOnly.ToString().ToLower();
            URL += "&offset=" + offset.ToString().ToLower();
            URL += "&limit=" + limit.ToString().ToLower();

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Authorization", "Ubi_v1 t=" + token);
            request.AddHeader("Ubi-SessionId", sessionId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<CatalogRoot>(response.Content);
            }
            return null;
        }
    }
}
