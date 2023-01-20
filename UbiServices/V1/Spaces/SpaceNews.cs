using Newtonsoft.Json.Linq;
using DalSoft.RestClient;

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

                Dictionary<string, string> headers = new();
                headers.Add("Ubi-AppId", V3.AppID);
                headers.Add("Ubi-LocaleCode", LocaleCode.ToString().Replace("_", "-"));

                var client = new RestClient(URL, headers);
                var posted = client.Get<JObject>();
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
        }
    }
}
