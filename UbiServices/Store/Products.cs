using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Store
{
    public partial class Products
    {
        /// <summary>
        /// Getting the Storefront data from productIds
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="products">List of productIds</param>
        /// <param name="expands">List of expandable</param>
        /// <param name="prodversion">Prod(Shop) Version</param>
        /// <returns>Json Object or Null</returns>
        public static JObject? GetStoreFrontByProducts(string storeType, List<string> products, List<string> expands, string prodversion = "v20_4")
        {
            string URL = $"https://store.ubi.com/s/{storeType}_uplaypc/dw/shop/{prodversion}/products/(";
            string URL_End = ")?client_id=2a3b13e8-a80b-4795-853a-4cd52645919b";

            if (products == null || products.Count == 0)
            {
                return null;
            }
            else
            {
                var productwithcommas = String.Join(",", products);
                URL += productwithcommas + URL_End;
            }

            if (expands == null || expands.Count == 0)
            {
                //Do nothing or debug print
            }
            else
            {
                var expandswithcommas = String.Join(",", expands);
                URL += "&expand=" + expandswithcommas;
            }

            var client = new RestClient(URL);
            var request = new RestRequest();

            RestResponse response = client.GetAsync(request).Result;
            if (response.Content != null)
            {
                return JObject.Parse(response.Content);
            }
            return null;
        }
    }
}
