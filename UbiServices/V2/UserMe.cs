using DalSoft.RestClient;
using UbiServices.Records;

namespace UbiServices.Public
{
    public partial class V2
    {
        public static readonly string URL_UsersMe = Urls.GetUrl("v2/users/me");
        /// <summary>
        /// Get User stuff from Ubi
        /// </summary>
        /// <param name="token">Ubi Token</param>
        /// <param name="sessionId">Session Id</param>
        /// <returns>v2UserMe or Null</returns>
        public static v2UserMe? GetUsersMe(string token, string sessionId)
        {
            Dictionary<string, string> headers = new();
            headers.Add("Authorization", $"Ubi_v1 t={token}");
            headers.Add("Ubi-AppId", V3.AppID);
            headers.Add("Ubi-SessionId", sessionId);

            var client = new RestClient(URL_UsersMe, headers);
            var posted = client.Get<v2UserMe>();
            posted.Wait();

            if (posted.Result.UserId == "")
                return null;

            return posted.Result;
        }
    }
}
