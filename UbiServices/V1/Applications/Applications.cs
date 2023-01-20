using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DalSoft.RestClient;
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
                
                Dictionary<string, string> headers = new();
                headers.Add("Ubi-AppId", ApplicationId);

                var client = new RestClient(URL, headers);
                var posted = client.Get<V1Applications>();
                posted.Wait();

                if (posted.Result.ApplicationId == "")
                    return null;

                return posted.Result;
            }
        }
    }
}
