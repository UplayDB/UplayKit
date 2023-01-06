using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Applications
        {
            public static readonly string URL_V1Applications = Urls.GetUrl("v1/applications/");
            /// <summary>
            /// Get Application Info
            /// </summary>
            /// <param name="ApplicationId">Ubi-AppId</param>
            /// <returns>V1Applications or Null</returns>
            public static V1Applications? GetApplications(string ApplicationId)
            {
                if (!Validations.IdValidation(ApplicationId))
                    return null;

                string URL = $"{URL_V1Applications}{ApplicationId}";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", ApplicationId);
                RestResponse response = client.GetAsync(request).Result;
                if (response.Content != null)
                {
                    return JsonConvert.DeserializeObject<V1Applications>(response.Content);
                }
                return null;
            }
        }
    }
}
