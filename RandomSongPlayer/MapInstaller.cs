using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaverSharp;

namespace RandomSongPlayer
{
    internal static class MapInstaller
    {
        internal static async Task<string> InstallMap(Beatmap mapData)
        {
            string mapPath = GetMapDirectoryName(mapData);

            byte[] zipData = await DownloadMap(mapData);
            if (!(zipData is null))
            {
                if (await ExtractZip(mapData, zipData, mapPath))
                {
                    return mapPath;
                }
            }
            return null;
        }

        private static async Task<byte[]> DownloadMap(Beatmap mapData)
        {
            try
            {
                byte[] zipData = await mapData.ZipBytes();
                return zipData;
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Unable to download map zip: " + ex.ToString());
                return null;
            }
            
        }

        private static string GetMapDirectoryName(Beatmap mapData)
        {
            return Setup.RandomSongsFolder + "/" + mapData.Key + " (" + mapData.Metadata.SongName + " - " + mapData.Metadata.LevelAuthorName + ")";
        }

        private static void UnzipFile(string fileName)
        {
            ZipFile.ExtractToDirectory(fileName + ".zip", fileName);
            File.Delete(fileName + ".zip");
        }

        private static async Task<bool> ExtractZip(Beatmap beatmap, byte[] zipData, string mapPath)
        {
            Stream zipStream = new MemoryStream(zipData);
            try
            {
                ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                string basePath = beatmap.Key + " (" + beatmap.Metadata.SongName + " - " + beatmap.Metadata.LevelAuthorName + ")";
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);
                await Task.Run(() =>
                {
                    foreach (var entry in archive.Entries)
                    {
                        var entryPath = Path.Combine(mapPath, entry.Name); // Name instead of FullName for better security and because song zips don't have nested directories anyway
                        if (!File.Exists(entryPath)) // Either we're overwriting or there's no existing file
                            entry.ExtractToFile(entryPath);
                    }
                }).ConfigureAwait(false);
                archive.Dispose();
                zipStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Unable to extract map zip: " + ex.ToString());
                zipStream.Close();
                return false;
            }
        }
    }
}
