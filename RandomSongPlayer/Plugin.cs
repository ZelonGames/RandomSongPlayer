using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using CustomUI.MenuButton;
using UnityEngine;
using System.Linq;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using UnityEngine.UI;
using SongCore;
using SongCore.Data;
using System.Collections.Generic;

namespace RandomSongPlayer
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static System.Random rnd = new System.Random();
        internal static HttpClient client = new HttpClient();
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;
        internal static SeperateSongFolder randomSongsFolder;

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            configProvider = cfgProvider;
                        
            randomSongsFolder = Collections.AddSeperateSongFolder("Random Songs", BeatSaber.InstallPath + "/" + Setup.RandomSongsFolder, FolderLevelPack.NewPack);

            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart");
            Setup.InstantiateData();
        }

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            try
            {
                if (scene.name == "MenuCore")
                {
                    MenuButtonUI.AddButton("Random Song Player", "Download a random song from Beat Saver and play it", () => { PlayRandomSongAsync(); });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:");
            }
        }

        private void OnPlayRandomSongClicked()
        {
            var mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            RandomSongMenu randomSongMenu = mainFlowCoordinator.gameObject.AddComponent<RandomSongMenu>();

            randomSongMenu.Show();
        }

        // this is just temporary ok!
        string path;
        public async void PlayRandomSongAsync()
        {
            await RandomSongGenerator.GenerateRandomKey(null);
            MapInstaller.InstallMap(RandomSongGenerator.mapData, out path);

            FileLogHelper.Log("Installed map");
            Loader.SongsLoadedEvent += OnLevelPacksRefreshed;

            path = Path.GetFullPath(path);

            Loader.Instance.RefreshSongs(false);
        }

        private void OnLevelPacksRefreshed(Loader loader, Dictionary<string, CustomPreviewBeatmapLevel> customPreviewBeatmapLevels)
        {
            FileLogHelper.Log("Levelpacks refreshed");

            CustomPreviewBeatmapLevel installedMap = randomSongsFolder.Levels[path];
            var difficulty = (BeatmapDifficulty)Enum.Parse(typeof(BeatmapDifficulty), installedMap.standardLevelInfoSaveData.difficultyBeatmapSets.First().difficultyBeatmaps.Last().difficulty);

            try
            {
                FileLogHelper.Log("Trying");

                LevelHelper.LoadBeatmapLevelAsync(installedMap, (status, success, beatmapLevel) =>
                {
                    
                    if (success)
                        LevelHelper.StartLevel(beatmapLevel, installedMap.beatmapCharacteristics.First(), difficulty);
                });
            }
            catch
            {

            }

            Loader.SongsLoadedEvent -= OnLevelPacksRefreshed;
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }
    }
}
