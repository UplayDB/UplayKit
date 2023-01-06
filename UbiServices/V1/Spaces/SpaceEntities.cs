using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Entities
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetSpaceEntities(string SpaceId)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}/entities";

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
