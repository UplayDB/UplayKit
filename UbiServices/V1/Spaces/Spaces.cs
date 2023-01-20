using DalSoft.RestClient;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V1
    {
        public partial class Spaces
        {
            /// <summary>
            /// Get Space Info
            /// </summary>
            /// <param name="SpaceId">Space Id</param>
            /// <returns>V1Spaces or Null</returns>
            public static V1Spaces? GetSpaces(string SpaceId)
            {
                if (!Validations.IdValidation(SpaceId))
                    return null;

                string URL = $"{URL_V1Spaces}{SpaceId}";
                Dictionary<string, string> headers = new();
                headers.Add("Ubi-AppId", V3.AppID);

                var client = new RestClient(URL, headers);
                var posted = client.Get<V1Spaces>();
                posted.Wait();

                if (posted.Result.DateCreated == DateTime.MinValue)
                    return null;

                return posted.Result;
            }
        }
    }
}
