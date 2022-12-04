using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V2
    {
        public partial class Applications
        {
            /// <summary>
            /// Get Application Info
            /// </summary>
            /// <param name="ApplicationId">Ubi-AppId</param>
            /// <returns>V2Applications or Null</returns>
            public static V2Applications? GetApplications(string ApplicationId)
            {
                string URL = $"https://public-ubiservices.ubi.com/v2/applications/{ApplicationId}";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", ApplicationId);
                RestResponse response = client.GetAsync(request).Result;
                if (response.Content != null)
                {
                    return JsonConvert.DeserializeObject<V2Applications>(response.Content);
                }
                return null;
            }
        }
    }
}
