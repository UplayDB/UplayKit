using Newtonsoft.Json.Linq;
using DalSoft.RestClient;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Battlepasses
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetSpaceBattlepasses(string SpaceId, string AuthTicket)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}/battlepasses";
                Dictionary<string, string> headers = new();
                headers.Add("AppId", $"Ubi_v1 t={AuthTicket}");
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
