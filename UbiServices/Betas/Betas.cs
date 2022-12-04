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
        public static BetasRoot? GetBetas(string AuthTicket)
        {
            string URL = $"https://beta.ubi.com/api/v1/betas";

            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", V3.AppID);
            request.AddHeader("Ubi-Ticket", AuthTicket);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<BetasRoot>(response.Content);
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
                return JsonConvert.DeserializeObject<BetasProfileRoot>(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="PlayerGroupId"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="ProfileId"></param>
        /// <param name="PlatfromId"></param>
        /// <param name="bodyJson"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
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
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    return JObject.Parse(response.Content);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="PlayerGroupId"></param>
        /// <param name="ProfileId"></param>
        /// <param name="PlatfromId"></param>
        /// <param name="bodyJson"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static JObject? JoinToBeta(string AuthTicket, string BetaCode, string PhaseId, string PlayerGroupId, string ProfileId, string PlatfromId, string bodyJson, string Method = "POST")
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
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    return JObject.Parse(response.Content);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="ProfileId"></param>
        /// <param name="PlatfromId"></param>
        /// <param name="FriendsId"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="PlayerGroupId"></param>
        /// <param name="ProfileId"></param>
        /// <param name="PlatfromId"></param>
        /// <param name="FriendsId"></param>
        /// <returns></returns>
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
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuthTicket"></param>
        /// <param name="ProfileId"></param>
        /// <param name="BetaCode"></param>
        /// <param name="PhaseId"></param>
        /// <param name="PlayerGroupId">Can be empty (use "" or String.Empty)</param>
        /// <param name="oldPlatform"></param>
        /// <param name="newPlatform"></param>
        /// <param name="Method">PUT or POST</param>
        /// <returns></returns>
        public static JObject? UpdatePlatform(string AuthTicket, string ProfileId, string BetaCode, string PhaseId, string PlayerGroupId, string oldPlatform, string newPlatform, string Method = "PUT")
        {
            string URL = $"https://beta.ubi.com/api/v1/{BetaCode}/phases/{PhaseId}/players/{ProfileId}";

            if (PlayerGroupId == "" || PlayerGroupId == string.Empty)
            {
                //Do nothing
            }
            else
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
                    return JObject.Parse(response.Content);
                }
            }
            else if (Method == "POST")
            {
                RestResponse response = client.PostAsync(request).Result;
                if (response.Content != null)
                {
                    return JObject.Parse(response.Content);
                }
            }

            return null;
        }
    }
}
