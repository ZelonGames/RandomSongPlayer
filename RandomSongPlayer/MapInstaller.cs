using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongPlayer
{
    internal static class MapInstaller
    {
        internal static void InstallMap(MapData mapData, out string path)
        {
            string mapDirectoryName = GetMapDirectoryName(mapData);
            path = mapDirectoryName;
            if (Directory.Exists(mapDirectoryName))
                return;

            DownloadMap(mapData);

            if (File.Exists(mapDirectoryName + ".zip"))
                UnzipFile(mapDirectoryName);
        }

        private static void DownloadMap(MapData mapData)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("https://beatsaver.com/api/download/key/" + mapData.key, GetMapDirectoryName(mapData) + ".zip");
            }
            catch (WebException we)
            {
                Console.WriteLine(we.ToString());
            }
        }

        private static string GetMapDirectoryName(MapData mapData)
        {
            return Setup.RandomSongsFolder + "/" + mapData.key;
        }

        private static void UnzipFile(string fileName)
        {
            ZipFile.ExtractToDirectory(fileName + ".zip", fileName);
            File.Delete(fileName + ".zip");
        }

        private static bool CanDownloadMap(string key, string directory, string fileName = null)
        {
            return !Directory.Exists(directory + fileName);
        }
    }
}
