using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UbiServices.Records;

namespace UbiServices.Store
{
    public static class AlgoliaSearch
    {
        /// <summary>
        /// Post Request datas to get the Search result<br/>
        /// IndexNames: (storetype)_product_suggestion,(storetype)_best_sellers_query_suggestions
        /// </summary>
        /// <param name="requests"></param>
        /// <returns>JObject or Null</returns>
        public static JObject? PostStoreAlgoliaSearch(List<Request> requests)
        {
            string URL = $"https://xely3u4lod-dsn.algolia.net/1/indexes/*/queries?x-algolia-agent=Algolia%20for%20JavaScript%20(3.35.1)%3B%20Browser&x-algolia-application-id=XELY3U4LOD&x-algolia-api-key=5638539fd9edb8f2c6b024b49ec375bd";
            var client = new RestClient(URL);
            var request = new RestRequest();

            var req = new RequestRoot(requests);
            request.AddBody(JsonConvert.SerializeObject(req));
            RestResponse response = client.PostAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
        }

        /// <summary>
        /// Query in Ubisoft Algolia Database via items
        /// </summary>
        /// <param name="storeType">ie,uk,us</param>
        /// <param name="productIds">List of productIds</param>
        /// <returns>JObject or Null</returns>
        public static JObject? PostStoreAlgoliaQuery(string storeType, List<string> productIds)
        {
            string URL = $"https://xely3u4lod-dsn.algolia.net/1/indexes/{storeType}_custom_MFE/query?x-algolia-agent=Algolia%20for%20JavaScript%20(4.8.5)%3B%20Browser&x-algolia-application-id=XELY3U4LOD&x-algolia-api-key=5638539fd9edb8f2c6b024b49ec375bd";
            var client = new RestClient(URL);
            var request = new RestRequest();

            request.AddBody("{\"query\":\"" + string.Join(",", productIds) + "\"}");
            RestResponse response = client.PostAsync(request).Result;
            if (response.Content != null)
            {
                Console.WriteLine(response.StatusCode);
                return JObject.Parse(response.Content);
            }
            return null;
        }
    }
}
