using Newtonsoft.Json;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V2
    {
        public partial class Applications
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ApplicationId"></param>
            /// <returns></returns>
            public static dynamic? GetApplicationConfig(string ApplicationId)
            {
                string URL = $"https://public-ubiservices.ubi.com/v2/applications/{ApplicationId}/configuration";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", ApplicationId);
                RestResponse response = client.GetAsync(request).Result;
                if (response.Content != null)
                {
                    return JsonConvert.DeserializeObject<dynamic>(response.Content);
                }
                return null;
            }
        }
    }
}
