using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Primary Store
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <param name="AuthTicket"></param>
            /// <param name="LocaleCode"></param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetSpacePrimarystore(string SpaceId, string AuthTicket, Enums.LocaleCode LocaleCode = Enums.LocaleCode.en_US)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}/configs/primarystore";

                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
                request.AddHeader("Authorization", $"Ubi_v1 t={AuthTicket}");
                request.AddHeader("Ubi-localeCode", LocaleCode.ToString().Replace("_", "-"));
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
