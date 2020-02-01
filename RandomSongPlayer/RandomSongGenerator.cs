using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomSongPlayer.Filter;
using Newtonsoft.Json;

namespace RandomSongPlayer
{
    internal static class RandomSongGenerator
    {
        internal static MapData mapData { get; private set; }

        private static List<string> difficulties = new List<string>();

        internal static async Task GenerateRandomKey(RandomKeyFilter randomKeyFilter)
        {
            // Define tries to find a map
            // TODO: Maybe make this setting not be hardcoded
            int tries = 0;
            int maxTries = 10;
            mapData = null;
            string randomKey = null;

            // Check for minimum requirements defined by user
            int? minRating = (randomKeyFilter != null && randomKeyFilter.Rating.HasValue)? (int?)(randomKeyFilter.Rating.Value * 100) : null;
            int? minNPS = (randomKeyFilter != null && randomKeyFilter.NPS.HasValue)? (int?)(randomKeyFilter.NPS.Value) : null;
            
            // Logging Purposes
            string logSearch = "Searching for random beatmap";
            if (minRating != null) logSearch += " with minimum rating of " + minRating.ToString();
            if (minNPS != null) logSearch += (minRating != null ? " and" : " with") + " minumum NPS of " + minNPS.ToString();
            logSearch += ".";
            Logger.log.Info(logSearch);

            // Look for the latest key on the Beatsaver API
            var responseString = await Plugin.client.GetStringAsync("https://beatsaver.com/api/maps/latest/0");
            var latestMaps = JsonConvert.DeserializeObject<Latest>(responseString);
            string latestKey = latestMaps.docs[0].key;
            int keyAsDecimal = int.Parse(latestKey, System.Globalization.NumberStyles.HexNumber);

            // Randomize the key and download the map
            if (randomKey == null)
            {
                // Check for Beatsaver Rating
                if (minRating != null)
                    await FilterHelper.SetFilterPageNumbers(Plugin.client, (int)minRating, "minRatingAPIPage");

                while (tries < maxTries && mapData == null)
                {
                    if (minRating != null)
                    {
                        await FilterHelper.SetRandomKey(Plugin.client, Plugin.rnd);
                        randomKey = FilterHelper.RandomKey;
                    }
                    else
                    {
                        int randomNumber = Plugin.rnd.Next(0, keyAsDecimal + 1);
                        randomKey = randomNumber.ToString("x");
                    }

                    await UpdateMapData(randomKeyFilter, randomKey);
                    tries++;
                }
            }
        }

        private static async Task UpdateMapData(RandomKeyFilter randomKeyFilter, string randomKey, bool showError = false)
        {
            try
            {
                var responseString = await Plugin.client.GetStringAsync("https://beatsaver.com/api/maps/detail/" + randomKey);

                mapData = JsonConvert.DeserializeObject<MapData>(responseString);

                #region NPS Filter
                Difficulty highestDifficulty = mapData.metadata.characteristics[0].difficulties.GetHighestDifficulty();
                if (randomKeyFilter != null && !randomKeyFilter.MatchingFilters(highestDifficulty, mapData))
                {
                    mapData = null;
                    return;
                }
                #endregion

                difficulties = mapData.metadata.difficulties.GetDifficulties();
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Logger.log.Info("Tried to download map with key '" + randomKey + "'. Map was most likely deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
