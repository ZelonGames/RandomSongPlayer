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
            int tries = 0;
            int maxTries = 10;

            mapData = null;

            string randomKey = null;


            var responseString = await Plugin.client.GetStringAsync("https://beatsaver.com/api/maps/latest/0");
            
            var latestMaps = JsonConvert.DeserializeObject<Latest>(responseString);
            string latestKey = latestMaps.docs[0].key;

            int keyAsDecimal = int.Parse(latestKey, System.Globalization.NumberStyles.HexNumber);

            if (randomKey == null)
            {
                if (randomKeyFilter != null && randomKeyFilter.Rating.HasValue)
                    await FilterHelper.SetFilterPageNumbers(Plugin.client, (int)(randomKeyFilter.Rating.Value * 100), "minRatingAPIPage");

                while (true)
                {
                    if (randomKeyFilter != null && randomKeyFilter.Rating.HasValue)
                    {
                        await FilterHelper.SetRandomKey(Plugin.client, Plugin.rnd);
                        randomKey = FilterHelper.RandomKey;
                    }
                    else
                    {
                        int randomNumber = Plugin.rnd.Next(0, keyAsDecimal + 1);
                        randomKey = randomNumber.ToString("x");
                    }

                    if (mapData == null)
                        await UpdateMapData(randomKeyFilter, randomKey);
                    if (mapData != null)
                        break;

                    if (tries == maxTries)
                        break;

                    tries++;
                }
            }

            if (tries == maxTries)
                mapData = null;
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
