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
using BeatSaverSharp;

namespace RandomSongPlayer
{
    internal static class RandomSongGenerator
    {
        internal static async Task<Beatmap> GenerateRandomKey()
        {
            int tries = 0;
            int maxTries = 20;
            string randomKey = null;
            Beatmap mapData = null;

            Logger.log.Info("Searching for random beatmap");

            // Look for the latest key on the Beatsaver API
            Page<PagedRequestOptions> latestMaps = await Plugin.beatsaverClient.Latest();
            string latestKey = latestMaps.Docs[0].Key;
            int keyAsDecimal = int.Parse(latestKey, System.Globalization.NumberStyles.HexNumber);

            // Randomize the key and download the map
            if (randomKey == null)
            {
                // Check for Beatsaver Rating
                // if (minRating != null)
                    // await FilterHelper.SetFilterPageNumbers(Plugin.client, (int)minRating, "minRatingAPIPage");

                while (tries < maxTries && mapData == null)
                {
                    int randomNumber = Plugin.rnd.Next(0, keyAsDecimal + 1);
                    randomKey = randomNumber.ToString("x");
                    mapData = await UpdateMapData(randomKey);
                    tries++;
                }
            }
            return mapData;
        }

        private static async Task<Beatmap> UpdateMapData(string randomKey)
        {
            try
            {
                Beatmap mapData = await Plugin.beatsaverClient.Key(randomKey);
                if (!(mapData is null))
                {
                    Logger.log.Info("Found map " + randomKey + ": " + mapData.Metadata.SongAuthorName + " - " + mapData.Metadata.SongName + " by " + mapData.Metadata.LevelAuthorName);
                }
                return mapData;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Logger.log.Info("Failed to download map with key '" + randomKey + "'. Map was most likely deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
