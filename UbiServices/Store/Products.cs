using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Store
{
    public partial class Products
    {
        /// <summary>
        /// Getting the Storefront data from productIds
        /// </summary>
        /// <param name="storeType">ie,uk,us</param>
        /// <param name="products">List of productIds</param>
        /// <returns>Json Object or Null</returns>
        public static JObject? GetStoreFrontByProducts(string storeType, List<string> products)
        {
            string URL = $"https://store.ubi.com/s/{storeType}_uplaypc/dw/shop/v19_8/products/(";
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
