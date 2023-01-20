using DalSoft.RestClient;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V2
    {
        public partial class Applications
        {
            public static readonly string URL_V2Applications = Urls.GetUrl("v2/applications/");

            /// <summary>
            /// Get Application Info
            /// </summary>
            /// <param name="ApplicationId">Ubi-AppId</param>
            /// <returns>V2Applications or Null</returns>
            public static V2Applications? GetApplications(string ApplicationId)
            {
                if (!Validations.IdValidation(ApplicationId))
                    return null;

                string URL = $"{URL_V2Applications}{ApplicationId}";
                Dictionary<string, string> headers = new();
                headers.Add("Ubi-AppId", ApplicationId);

                var client = new RestClient(URL, headers);
                var posted = client.Get<V2Applications>();
                posted.Wait();

                if (posted.Result.ApplicationId == "")
                    return null;

                return posted.Result;
            }
        }
    }
}
