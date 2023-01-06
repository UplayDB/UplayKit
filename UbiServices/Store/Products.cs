using Newtonsoft.Json.Linq;
using RestSharp;

namespace UbiServices.Store
{
    public partial class Products
    {
        /// <summary>
        /// Getting the Storefront data from productIds
        /// </summary>
        /// <param name="countrycode">Country Code</param>
        /// <param name="products">List of productIds</param>
        /// <param name="expands">List of expandable</param>
        /// <param name="IsPC">Ubisoft or UplayPC</param>
        /// <param name="prodversion">Prod(Shop) Version</param>
        /// <returns>JObject or Null</returns>
        public static JObject? GetStoreFrontByProducts(Enums.CountryCode countrycode, List<string> products, List<string> expands, bool IsPC = true, Enums.StoreVersion prodversion = Enums.StoreVersion.v22_10)
        {
            string store = "";
            if (IsPC)
            {
                store = "_uplaypc";
            }
            else
            {
                store = "_ubisoft";
            }
            string URL = $"https://store.ubi.com/s/{countrycode.ToString()}{store}/dw/shop/{prodversion.ToString()}/products/(";
            string URL_End = ")?client_id=2a3b13e8-a80b-4795-853a-4cd52645919b";

            if (products == null || products.Count == 0)
            {
                return null;
            }
            else if (products.Count == 1)
            {
                URL += products[0] + URL_End;
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

            //Console.WriteLine(URL);
            var client = new RestClient(URL);
            var request = new RestRequest();

            try
            {
                RestResponse response = client.Get(request);
                if (response.Content != null)
                {
                    return JObject.Parse(response.Content);
                }
            }
            catch { }

            return null;
        }
    }
}
