using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V2
    {
        public partial class Applications
        {
            /// <summary>
            /// Get Application Configuration
            /// </summary>
            /// <param name="ApplicationId">Ubi-AppId</param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetApplicationConfig(string ApplicationId)
            {
                if (!Validations.IdValidation(ApplicationId))
                    return null;

                string URL = $"{URL_V2Applications}{ApplicationId}/configuration";
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
