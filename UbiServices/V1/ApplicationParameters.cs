﻿using Newtonsoft.Json;
using RestSharp;

namespace UbiServices
{
    partial class V1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <returns></returns>
        public static dynamic? GetApplicationParameters(string ApplicationId)
        {
            string URL = $"https://public-ubiservices.ubi.com/v1/applications/{ApplicationId}/parameters";
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddHeader("Ubi-AppId", ApplicationId);
            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JsonConvert.DeserializeObject<dynamic>(response.Content);
            }
            return null;
        }
    }
}