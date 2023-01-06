using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Info
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <returns>V1Spaces or Null</returns>
            public static V1Spaces? GetSpaces(string SpaceId)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
                RestResponse response = client.GetAsync(request).Result;
                if (response.Content != null)
                {
                    return JsonConvert.DeserializeObject<V1Spaces>(response.Content);
                }
                return null;
            }
        }
    }
}
