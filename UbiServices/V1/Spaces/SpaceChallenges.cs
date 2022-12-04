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
            /// <returns></returns>
            public static JObject? GetSpaceChallenges(string SpaceId)
            {
                string URL = $"https://public-ubiservices.ubi.com/v1/spaces/{SpaceId}/challenges";

                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
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
