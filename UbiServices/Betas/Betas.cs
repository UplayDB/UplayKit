using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DalSoft.RestClient;
using UbiServices.Public;
using UbiServices.Records;
using System.Reflection.PortableExecutable;

namespace UbiServices
{
    public class Betas
    {
        /// <summary>
        /// Get All public Betas
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <returns>BetasRoot or Null</returns>
        public static List<BetasRoot>? GetBetas(string AuthTicket)
        {
            string URL = $"https://beta.ubi.com/api/v1/betas";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<List<BetasRoot>>();
            posted.Wait();

            if (posted.Result.Count == 0)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get all Betas from Profile
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="ProfileId">Profile Id</param>
        /// <returns>BetasProfileRoot or Null</returns>
        public static BetasProfileRoot? GetBetasProfile(string AuthTicket, string ProfileId)
        {
            string URL = $"https://beta.ubi.com/api/v1/profiles/{ProfileId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<BetasProfileRoot>();
            posted.Wait();

            if (posted.Result.CountryCode == "")
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get Beta by BetaCode
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetBetasByCode(string AuthTicket, string BetaCode)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get phases by BetaCode
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetBetasPhases(string AuthTicket, string BetaCode)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get Data from Phase by Id and BetaCode
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetBetasPhase(string AuthTicket, string BetaCode, string PhaseId)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get Playergroups from Phase by ID and BetaCode
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetBetasPhasePlayergroups(string AuthTicket, string BetaCode, string PhaseId)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/playergroups";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Get Data from Groups by Ids and BetaCode
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="PlayerGroupId">Group Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetBetasPhasePlayergroup(string AuthTicket, string BetaCode, string PhaseId, string PlayerGroupId)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/playergroups/{PlayerGroupId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Get<JObject>();
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Join the Beta with provided information
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="ProfileId">Profile Id (Same as the token author)</param>
        /// <param name="PlatfromId">Platform Id (usually 1)</param>
        /// <param name="bodyJson">-</param>
        /// <param name="Method">PUT or POST</param>
        /// <returns>JObject or Null</returns>
        public static JObject? JoinToBeta(string AuthTicket, string BetaCode, string PhaseId, string ProfileId, string PlatfromId, string bodyJson, string Method = "POST")
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}?platformId={PlatfromId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);

            if (Method == "PUT")
            {
                var posted = client.Put<string,JObject>(bodyJson);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
            else if (Method == "POST")
            {
                var posted = client.Post<string, JObject>(bodyJson);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
            return null;
        }

        /// <summary>
        /// Join the Beta with provided information
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="PlayerGroupId">Group Id</param>
        /// <param name="ProfileId">Profile Id (Same as the token author)</param>
        /// <param name="PlatfromId">Platform Id (usually 1)</param>
        /// <param name="bodyJson">-</param>
        /// <param name="Method">PUT or POST</param>
        /// <returns>JObject or Null</returns>
        public static JObject? JoinToBetaGroup(string AuthTicket, string BetaCode, string PhaseId, string PlayerGroupId, string ProfileId, string PlatfromId, string bodyJson, string Method = "POST")
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}/playergroups/{PlayerGroupId}?platformId={PlatfromId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);

            if (Method == "PUT")
            {
                var posted = client.Put<string, JObject>(bodyJson);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
            else if (Method == "POST")
            {
                var posted = client.Post<string, JObject>(bodyJson);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
            return null;
        }

        /// <summary>
        /// Send Friend Invite the Beta with provided information
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="ProfileId">Profile Id (Same as the token author)</param>
        /// <param name="PlatfromId">Platform Id (usually 1)</param>
        /// <param name="FriendsId">Friend Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? PostFriendInviteToBeta(string AuthTicket, string BetaCode, string PhaseId, string ProfileId, string PlatfromId, string FriendsId)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}/friends?platformId={PlatfromId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Post<string, JObject>("{\"Friends\":[{\"FriendId\":\"" + FriendsId + "\"}]}");
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Send Friend Invite the Beta with provided information
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="PlayerGroupId">Group Id</param>
        /// <param name="ProfileId">Profile Id (Same as the token author)</param>
        /// <param name="PlatfromId">Platform Id (usually 1)</param>
        /// <param name="FriendsId">Friend Id</param>
        /// <returns>JObject or Null</returns>
        public static JObject? PostFriendInviteToBeta(string AuthTicket, string BetaCode, string PhaseId, string PlayerGroupId, string ProfileId, string PlatfromId, string FriendsId)
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}/playergroups/{PlayerGroupId}/friends?platformId={PlatfromId}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);
            var posted = client.Post<string, JObject>("{\"Friends\":[{\"FriendId\":\"" + FriendsId + "\"}]}");
            posted.Wait();

            if (posted.Result.HasValues == false)
                return null;

            return posted.Result;
        }

        /// <summary>
        /// Update user platform choice
        /// </summary>
        /// <param name="AuthTicket">Ubi Token</param>
        /// <param name="ProfileId">Profile Id (Same as the token author)</param>
        /// <param name="BetaCode">Beta Code</param>
        /// <param name="PhaseId">Phase Id</param>
        /// <param name="PlayerGroupId">Can be empty (use "" or String.Empty)</param>
        /// <param name="oldPlatform">Platform Id</param>
        /// <param name="newPlatform">Platform Id</param>
        /// <param name="Method">PUT or POST</param>
        /// <returns>JObject or Null</returns>
        public static JObject? UpdatePlatform(string AuthTicket, string ProfileId, string BetaCode, string PhaseId, string PlayerGroupId, string oldPlatform, string newPlatform, string Method = "PUT")
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}";

            if (PlayerGroupId != "" || !string.IsNullOrEmpty(PlayerGroupId))
            {
                URL += $"playergroups/{PlayerGroupId}";
            }

            URL += $"?platformId={oldPlatform}&newPlatformId={newPlatform}";

            Dictionary<string, string> headers = new();
            headers.Add("Ubi-Ticket", AuthTicket);
            headers.Add("Ubi-AppId", V3.AppID);

            var client = new RestClient(URL, headers);

            string body = "{}";

            if (Method == "PUT")
            {
                var posted = client.Put<string, JObject>(body);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }
            else if (Method == "POST")
            {
                var posted = client.Post<string, JObject>(body);
                posted.Wait();

                if (posted.Result.HasValues == false)
                    return null;

                return posted.Result;
            }

            return null;
        }
    }
}
