using Newtonsoft.Json.Linq;
using DalSoft.RestClient;
using UbiServices.Records;

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

                Dictionary<string, string> headers = new();
                headers.Add("Authorization", $"Ubi_v1 t={AuthTicket}");
                headers.Add("Ubi-LocaleCode", LocaleCode.ToString().Replace("_", "-"));
                headers.Add("Ubi-AppId", V3.AppID);

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
