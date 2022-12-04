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
            /// <param name="AuthTicket"></param>
            /// <param name="LocaleCode"></param>
            /// <returns></returns>
            public static JObject? GetSpacePrimarystore(string SpaceId, string AuthTicket, string LocaleCode = "en-US")
            {
                string URL = $"https://public-ubiservices.ubi.com/v1/spaces/{SpaceId}/configs/primarystore";

                var client = new RestClient(URL);
                var request = new RestRequest();

                request.AddHeader("Ubi-AppId", V3.AppID);
                request.AddHeader("Authorization", $"Ubi_v1 t={AuthTicket}");
                request.AddHeader("Ubi-localeCode", LocaleCode);
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
