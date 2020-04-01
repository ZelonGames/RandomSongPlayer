using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using IPA;
using IPA.Loader;
using IPA.Config;
using IPA.Config.Stores;
using BS_Utils.Utilities;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using UnityEngine;
using System.Linq;
using SongCore;
using SongCore.Data;
using RandomSongPlayer.UI;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils;
using BeatSaverSharp;
using System.Reflection;

namespace RandomSongPlayer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static System.Random rnd = new System.Random();
        internal static BeatSaver beatsaverClient = new BeatSaver(new HttpOptions() { ApplicationName = Assembly.GetExecutingAssembly().GetName().Name, Version = Assembly.GetExecutingAssembly().GetName().Version });
        internal static PluginConfig config;
        internal static SeperateSongFolder randomSongsFolder;
        public static Plugin instance;

        public static IAnnotatedBeatmapLevelCollection Playlist
        {
            get { return randomSongsFolder.LevelPack; }
        }

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config cfgProvider, PluginMetadata pluginMetadata)
        {
            instance = this;
            Logger.log = logger;

            Sprite coverImage = SongCore.Utilities.Utils.LoadSpriteFromResources("RandomSongPlayer.Assets.new-rst-logo.png");
            randomSongsFolder = Collections.AddSeperateSongFolder("Random Songs", Environment.CurrentDirectory + "/" + Setup.RandomSongsFolder, FolderLevelPack.NewPack, coverImage);

            config = cfgProvider.Generated<PluginConfig>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Logger.log.Info("OnApplicationStart");
            BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += BSEvents_menuSceneLoadedFresh;
        }

        private void _levelFilteringNavController_didSelectPackEvent(LevelFilteringNavigationController levelFilteringNavigationController, IAnnotatedBeatmapLevelCollection iAnnotatedBeatmapLevelCollection, GameObject gameObject, BeatmapCharacteristicSO beatmapCharacteristicSO)
        {
            IBeatmapLevelPack levelPack = iAnnotatedBeatmapLevelCollection as IBeatmapLevelPack;
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

            try
            {
                MenuButton menuButton = new MenuButton("Random Song Player", "Download a random song from Beat Saver and play it", async () => { await PlayRandomSongAsync(); });
                MenuButtons.instance.RegisterButton(menuButton);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
        }



        public delegate void RSPDownloadedCallback(CustomPreviewBeatmapLevel chosenSong);

        /** If the callback is null, call the callback with the randomSongId, otherwise select the newly downloaded song */
        public async Task DownloadRandomSongAsync(RSPDownloadedCallback callback)
        {
            Beatmap mapData = await RandomSongGenerator.GenerateRandomKey();
            if (!(mapData is null))
            {
                string path = await MapInstaller.InstallMap(mapData);
                Logger.log.Info("Chosen Random Song: " + path);

                path = Path.GetFullPath(path);
                // Have fun with this.
                Action OnLevelPacksRefreshed = null;
                OnLevelPacksRefreshed = () =>
                {
                    Loader.OnLevelPacksRefreshed -= OnLevelPacksRefreshed;
                    CustomPreviewBeatmapLevel installedMap = randomSongsFolder.Levels[path];
                    callback?.Invoke(installedMap);
                };
                Loader.OnLevelPacksRefreshed += OnLevelPacksRefreshed;
                Loader.Instance.RefreshSongs(false);
            }
        }

        public async Task SelectRandomSongAsync()
        {
            await DownloadRandomSongAsync(OnLevelPacksRefreshedSelect);
        }

        public async Task PlayRandomSongAsync()
        {
            await DownloadRandomSongAsync(OnLevelPacksRefreshedPlay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="installedMap"></param>
        private void OnLevelPacksRefreshedSelect(CustomPreviewBeatmapLevel installedMap)
        {
            int installedLevelIndex = Array.FindIndex(randomSongsFolder.LevelPack.beatmapLevelCollection.beatmapLevels, x => (x.levelID == installedMap.levelID));

            LevelCollectionTableView levelCollectionTable = Resources.FindObjectsOfTypeAll<LevelCollectionTableView>().First();
            var tableView = levelCollectionTable.GetPrivateField<HMUI.TableView>("_tableView");
            tableView.ScrollToCellWithIdx(installedLevelIndex + 1, HMUI.TableViewScroller.ScrollPositionType.Center, true);
            tableView.SelectCellWithIdx(installedLevelIndex + 1, true);
        }

        private void OnLevelPacksRefreshedPlay(CustomPreviewBeatmapLevel installedMap)
        {
            var difficulty = (BeatmapDifficulty)Enum.Parse(typeof(BeatmapDifficulty), installedMap.standardLevelInfoSaveData.difficultyBeatmapSets.First().difficultyBeatmaps.Last().difficulty);

            LevelHelper.PlayLevel(installedMap, difficulty);
        }
    }
}
