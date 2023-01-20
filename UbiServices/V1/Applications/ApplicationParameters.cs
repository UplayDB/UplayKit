using Newtonsoft.Json.Linq;
using DalSoft.RestClient;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Applications
        {
            /// <summary>
            /// Get Application Parameters
            /// </summary>
            /// <param name="ApplicationId">Ubi-AppId</param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetApplicationParameters(string ApplicationId)
            {
                if (!Validations.IdValidation(ApplicationId))
                    return null;

                string URL = $"{URL_V1Applications}{ApplicationId}/parameters";
                Dictionary<string, string> headers = new();
                headers.Add("Ubi-AppId", ApplicationId);

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
