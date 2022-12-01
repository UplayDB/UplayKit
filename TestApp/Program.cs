using Newtonsoft.Json.Linq;
using System;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<UbiServices.Records.Request> requests = new();
            requests.Add(
                new()
                {   IndexName = "ie_product_suggestion",
                    Params = "hitsPerPage=100&page=0&highlightPreTag=__ais-highlight__&highlightPostTag=__%2Fais-highlight__&facets=%5B%5D&tagFilters=&analytics=false"
                }
            );

            


            var result = UbiServices.Store.AlgoliaSearch.PostStoreAlgoliaSearch(requests);

            if (result != null)
            {
                JArray resultArray = (JArray)result["results"];

                for (int i = 0; i < resultArray.Count; i++)
                {
                    JObject resultsAObject = JObject.FromObject(resultArray[i]);
                    JArray resultsAHits = (JArray)resultsAObject["hits"];
                    //Console.WriteLine(o2);
                    for (int i2 = 0; i2 < resultsAHits.Count; i2++)
                    {
                        JObject finalJson = JObject.FromObject(resultsAHits[i2]);
                        JValue title = (JValue)finalJson["title"];
                        JValue id = (JValue)finalJson["id"];
                        Console.WriteLine($"{title} ({id})");
                    }
                }
            }


            Console.WriteLine();
        }
    }
}
