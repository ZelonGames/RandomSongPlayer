using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RandomSongPlayer;

namespace RandomSongPlayer.Filter
{
    internal static class FilterHelper
    {
        public static int FilterPageNumber { get; private set; }
        public static string RandomKey { get; private set; }

        /*internal static async Task SetFilterPageNumbers(HttpClient client, int minValue, string dataTypeName)
        {
            #region Get FilterData

            FilterData filterData = null;

            string filterDataFileName = "UserData/filterData.json";
            string filterDataJson = null;

            if (File.Exists(filterDataFileName))
            {
                filterDataJson = File.ReadAllText(filterDataFileName);
                filterData = JsonConvert.DeserializeObject<FilterData>(filterDataJson);
            }

            #endregion

            Latest ratingMaps = null;

            int? currentPage = null;

            #region Set last page

            string ratingString = null;

            if (filterData != null && filterData.filterData.ContainsKey(dataTypeName))
            {
                if (filterData.filterData[dataTypeName].ContainsKey(minValue))
                    currentPage = filterData.filterData[dataTypeName][minValue];
                else
                {
                    int closestValue = filterData.filterData[dataTypeName].First().Value;
                    int closestDifference = int.MaxValue;

                    foreach (var key in filterData.filterData[dataTypeName].Keys)
                    {
                        int difference = Math.Abs(minValue - key);
                        if (difference < closestDifference)
                        {
                            closestDifference = difference;
                            closestValue = filterData.filterData[dataTypeName][key];
                        }
                    }

                    currentPage = closestValue;
                }
            }

            #endregion

            ratingString = await client.GetStringAsync("https://beatsaver.com/api/maps/rating/0");
            ratingMaps = JsonConvert.DeserializeObject<Latest>(ratingString);
            int lastPage = ratingMaps.lastPage;

            if (!currentPage.HasValue)
                currentPage = lastPage;

            int maxPage = currentPage.Value;
            int minPage = 0;

            int maxTries = 100;
            int tries = 0;

            while (true)
            {
                ratingString = await client.GetStringAsync("https://beatsaver.com/api/maps/rating/" + currentPage);
                ratingMaps = JsonConvert.DeserializeObject<Latest>(ratingString);

                double value = ratingMaps.docs.First().stats.rating;

                if (Math.Abs(value - minValue) <= 0.5)
                    break;

                if (maxPage == currentPage && value > minValue)
                {
                    maxPage = lastPage;
                    minPage = currentPage.Value;
                }

                if (value < minValue)
                {
                    maxPage = currentPage.Value;
                    currentPage = (minPage + maxPage) / 2;
                }
                else if (value > minValue)
                {
                    minPage = currentPage.Value;
                    currentPage = (minPage + maxPage) / 2;
                }

                if (maxPage - minPage <= 1)
                    break;

                tries++;

                if (tries >= maxTries)
                    break;
            }

            FilterPageNumber = currentPage.Value;

            #region Update and Save FilterData

            if (!File.Exists(filterDataFileName))
            {
                File.Create(filterDataFileName).Close();

                var ratings = new Dictionary<string, Dictionary<int, int>>();
                ratings.Add(dataTypeName, new Dictionary<int, int>());
                ratings[dataTypeName].Add(minValue, currentPage.Value);
                filterData = new FilterData(ratings);
            }
            else
            {
                filterDataJson = File.ReadAllText(filterDataFileName);
                filterData = JsonConvert.DeserializeObject<FilterData>(filterDataJson);
            }

            if (!filterData.filterData.ContainsKey(dataTypeName))
                filterData.filterData.Add(dataTypeName, new Dictionary<int, int>());

            if (filterData.filterData[dataTypeName].ContainsKey(minValue))
                filterData.filterData[dataTypeName][minValue] = currentPage.Value;
            else
                filterData.filterData[dataTypeName].Add(minValue, currentPage.Value);

            filterDataJson = JsonConvert.SerializeObject(filterData);

            File.WriteAllText(filterDataFileName, filterDataJson);

            #endregion
        }

        internal static async Task SetRandomKey(HttpClient client, Random rnd)
        {
            var ratingString = await client.GetStringAsync("https://beatsaver.com/api/maps/rating/" + rnd.Next(0, FilterPageNumber + 1));
            var ratingMaps = JsonConvert.DeserializeObject<Latest>(ratingString);

            RandomKey = ratingMaps.docs[rnd.Next(0, ratingMaps.docs.Count)].key;
        }*/
    }
}
