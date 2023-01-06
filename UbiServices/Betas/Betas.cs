using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UbiServices.Public;
using UbiServices.Records;

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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JsonConvert.DeserializeObject<List<BetasRoot>>(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JsonConvert.DeserializeObject<BetasProfileRoot>(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);

            request.AddBody(bodyJson);

            if (Method == "PUT")
            {
                RestResponse response = client.PutAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);

            request.AddBody(bodyJson);

            if (Method == "PUT")
            {
                RestResponse response = client.PutAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);

            request.AddBody("{\"Friends\":[{\"FriendId\":\"" + FriendsId + "\"}]}");

            RestResponse response = client.PostAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);

            request.AddBody("{\"Friends\":[{\"FriendId\":\"" + FriendsId + "\"}]}");

            RestResponse response = client.PostAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
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

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);

            request.AddBody("{}");

            if (Method == "PUT")
            {
                RestResponse response = client.PutAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    Console.WriteLine(response.StatusCode);
                    return JObject.Parse(response.Content);
                }
            }

            return null;
        }
    }
}
