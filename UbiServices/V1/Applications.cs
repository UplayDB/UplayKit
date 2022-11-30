using Newtonsoft.Json;
using RestSharp;
using UbiServices.Records;

namespace UbiServices
{
    public partial class V1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <returns></returns>
        public static V1Applications? GetApplications(string ApplicationId)
        {
            string URL = $"https://public-ubiservices.ubi.com/v1/applications/{ApplicationId}";
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", ApplicationId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<V1Applications>(response.Content);
            }
            return null;
        }
    }
}
