using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Applications
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ApplicationId"></param>
            /// <returns></returns>
            public static JObject? GetApplicationConfig(string ApplicationId)
            {
                string URL = $"https://public-ubiservices.ubi.com/v1/applications/{ApplicationId}/configuration";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", ApplicationId);
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
