using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SpaceId"></param>
            /// <param name="localeCode"></param>
            /// <returns></returns>
            public static JObject? GetNews(string SpaceId, string localeCode = "en-US")
            {
                string URL = $"https://msr-public-ubiservices.ubi.com/v1/spaces/news?spaceId={SpaceId}";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
                request.AddHeader("Ubi-localeCode", localeCode);

                RestResponse response = client.GetAsync(request).Result;
                if (response.Content != null)
                {
                    return JObject.Parse(response.Content);
                }
                return null;
            }
        }
    }
}
