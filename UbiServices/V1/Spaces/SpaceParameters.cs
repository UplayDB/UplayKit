using Newtonsoft.Json.Linq;
using DalSoft.RestClient;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Parameters
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <param name="ParameterGroup">Filter by ParameterGroup</param>
            /// <returns>JObject or Null</returns>
            public static JObject? GetSpaceParameters(string SpaceId, string ParameterGroup = "")
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}/parameters";

                if (ParameterGroup != "")
                {
                    URL += "?parameterGroups=" + ParameterGroup;
                }

                Dictionary<string, string> headers = new();
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
