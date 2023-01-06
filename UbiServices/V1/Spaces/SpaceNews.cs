using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get News from Space
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <param name="LocaleCode"></param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetNews(string SpaceId, Enums.LocaleCode LocaleCode = Enums.LocaleCode.en_US)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}/news?spaceId={SpaceId}";
                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
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
