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
using UnityEngine;
using System.Linq;
using SongCore;
using SongCore.Data;
using RandomSongPlayer.UI;

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
            // TODOKETE
            Sprite coverImage = SongCore.Utilities.Utils.LoadSpriteFromResources("RandomSongPlayer.Assets.random-song-tourney-icon.png");

            randomSongsFolder = Collections.AddSeperateSongFolder("Random Songs", BeatSaber.InstallPath + "/" + Setup.RandomSongsFolder, FolderLevelPack.NewPack, coverImage);
            
            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        public void OnApplicationStart()
        {
            Logger.log.Info("OnApplicationStart");
            BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += BSEvents_menuSceneLoadedFresh;
        }

        private void _levelFilteringNavController_didSelectPackEvent(LevelFilteringNavigationController arg1, IAnnotatedBeatmapLevelCollection arg2, GameObject arg3, BeatmapCharacteristicSO arg4)
        {
            IBeatmapLevelPack levelPack = arg2 as IBeatmapLevelPack;
            if (levelPack == null || levelPack.packName != "Random Songs")
            {
                Logger.log.Info("Hiding RandomSongButton");
                RandomButtonUI.instance.Hide();
                return;
            }
            else
            {
                Logger.log.Info("Showing RandomSongButton");
                RandomButtonUI.instance.Show();
            }
        }

        private void BSEvents_menuSceneLoadedFresh()
        {
            LevelFilteringNavigationController levelFiltering = Resources.FindObjectsOfTypeAll<LevelFilteringNavigationController>().First();
            levelFiltering.didSelectAnnotatedBeatmapLevelCollectionEvent -= _levelFilteringNavController_didSelectPackEvent;
            levelFiltering.didSelectAnnotatedBeatmapLevelCollectionEvent += _levelFilteringNavController_didSelectPackEvent;
            RandomButtonUI.instance.Setup(this);
        }

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
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
                    // BeatSaberMarkupLanguage.MenuButtons.MenuButton menuButton = 
                    // TODOKETE
                    // MenuButtonUI.AddButton("Random Song Player", "Download a random song from Beat Saver and play it", () => { PlayRandomSongAsync(); });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:");
            }
        }

        // this is just temporary ok!
        string path;
        public async Task DownloadRandomSongAsync()
        {
            await RandomSongGenerator.GenerateRandomKey(null);
            MapInstaller.InstallMap(RandomSongGenerator.mapData, out path);

            Loader.OnLevelPacksRefreshed += OnLevelPacksRefreshed;

            path = Path.GetFullPath(path);
            Logger.log.Info("Path: " + path);

            Loader.Instance.RefreshSongs(false);
        }

        private void OnLevelPacksRefreshed()
        {
            CustomPreviewBeatmapLevel installedMap = randomSongsFolder.Levels[path];            
            int installedLevelIndex = Array.FindIndex(randomSongsFolder.LevelPack.beatmapLevelCollection.beatmapLevels, x => (x.levelID == installedMap.levelID));

            LevelCollectionTableView levelCollectionTable = Resources.FindObjectsOfTypeAll<LevelCollectionTableView>().First();
            var tableView = levelCollectionTable.GetPrivateField<HMUI.TableView>("_tableView");
            tableView.ScrollToCellWithIdx(installedLevelIndex+1, HMUI.TableViewScroller.ScrollPositionType.Center, true);
            tableView.SelectCellWithIdx(installedLevelIndex + 1, true);
                        
            Loader.OnLevelPacksRefreshed -= OnLevelPacksRefreshed;
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}
