using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V1
    {
        /// <summary>
        /// Get Spaces Catalog
        /// </summary>
        /// <param name="token">Ubi Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <param name="defaultOnly">Get the default or not</param>
        /// <param name="offset">Offset where to start from</param>
        /// <param name="limit">Limited number of category</param>
        /// <returns>CatalogRoot or Null</returns>
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
