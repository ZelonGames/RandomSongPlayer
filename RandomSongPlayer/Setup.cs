using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    internal static class Setup
    {
        internal static string RandomSongsFolder => "Beat Saber_Data/Random Songs";
        internal static string DataFolderName => "UserData/RandomSongPlayerData";
        internal static string MapDataFileName => DataFolderName + "/mapData.json";

        internal static void InstantiateData()
        {
            try
            {
                if (!Directory.Exists(DataFolderName))
                    Directory.CreateDirectory(DataFolderName);

                if (!File.Exists(MapDataFileName))
                    File.Create(MapDataFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
