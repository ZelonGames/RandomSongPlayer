using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using Newtonsoft.Json;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage;

namespace RandomSongPlayer
{
    internal static class LevelHelper
    {
        internal static void PlayLevel(CustomPreviewBeatmapLevel beatmap, BeatmapDifficulty difficulty)
        {

            try
            {
                LoadBeatmapLevelAsync(beatmap, (success, beatmapLevel) =>
                {
                    Logger.log.Log(IPALogger.Level.Info, "Loading Beatmap level Success:" + success);
                    if (success)
                    {
                        StartLevel(beatmapLevel, beatmap, difficulty);
                    }

                });
            }
            catch (Exception ex)
            {
                Logger.log.Log(IPALogger.Level.Critical, ex);
            }
        }

        private static void StartLevel(IBeatmapLevel beatmapLevel, CustomPreviewBeatmapLevel beatmap, BeatmapDifficulty difficulty)
        {
            Logger.log.Info("Starting level");

            MenuTransitionsHelper menuSceneSetupData = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault();
            PlayerData playerSettings = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault().playerData;

            var gamePlayModifiers = new GameplayModifiers();
            gamePlayModifiers.IsWithoutModifiers();

            IBeatmapLevel level = beatmapLevel;
            BeatmapCharacteristicSO characteristics = beatmap.previewDifficultyBeatmapSets[0].beatmapCharacteristic;
            IDifficultyBeatmap levelDifficulty = BeatmapLevelDataExtensions.GetDifficultyBeatmap(level.beatmapLevelData, characteristics, difficulty);
            menuSceneSetupData.StartStandardLevel(levelDifficulty,
                playerSettings.overrideEnvironmentSettings.overrideEnvironments ? playerSettings.overrideEnvironmentSettings : null,
                playerSettings.colorSchemesSettings.overrideDefaultColors ? playerSettings.colorSchemesSettings.GetSelectedColorScheme() : null,
                gamePlayModifiers,
                playerSettings.playerSpecificSettings,
                null, "Exit", playerSettings.playerSpecificSettings.sfxVolume > 0, () => { }, (StandardLevelScenesTransitionSetupDataSO sceneTransition, LevelCompletionResults results) =>
                {
                    bool newHighScore = false;

                    var mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                    RandomSongMenu randomSongMenu = BeatSaberUI.CreateFlowCoordinator<RandomSongMenu>();

                    if (results.levelEndAction == LevelCompletionResults.LevelEndAction.Restart)
                    {
                        Logger.log.Info("Restarting level");
                        PlayLevel(JsonConvert.DeserializeObject<CustomPreviewBeatmapLevel>(JsonConvert.SerializeObject(beatmap)), JsonConvert.DeserializeObject<BeatmapDifficulty>(JsonConvert.SerializeObject(difficulty)));
                        return;
                    }

                    switch (results.levelEndStateType)
                    {
                        case LevelCompletionResults.LevelEndStateType.None:
                            break;
                        case LevelCompletionResults.LevelEndStateType.Cleared:
                            UploadScore(levelDifficulty, results, out newHighScore);
                            randomSongMenu.Show(beatmap, difficulty, levelDifficulty, results, newHighScore);
                            Logger.log.Info("Showing menu");
                            break;
                        case LevelCompletionResults.LevelEndStateType.Failed:
                            Logger.log.Info("Showing menu");
                            randomSongMenu.Show(beatmap, difficulty, levelDifficulty, results, newHighScore);
                            break;
                        default:
                            break;
                    }
                });
        }

        private static async void LoadBeatmapLevelAsync(IPreviewBeatmapLevel selectedLevel, Action<bool, IBeatmapLevel> callback)
        {
            var token = new CancellationTokenSource();

            var _beatmapLevelsModel = Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().FirstOrDefault();
            Logger.log.Info("Level ID: " + selectedLevel.levelID);

            var _loadedPreviewBeatmapLevels = _beatmapLevelsModel.GetPrivateField<Dictionary<string, IPreviewBeatmapLevel>>("_loadedPreviewBeatmapLevels");
            bool containsKey = _loadedPreviewBeatmapLevels.ContainsKey(selectedLevel.levelID);

            if (!containsKey)
                _loadedPreviewBeatmapLevels.Add(selectedLevel.levelID, selectedLevel);

            Logger.log.Info("Has key: " + containsKey);

            BeatmapLevelsModel.GetBeatmapLevelResult getBeatmapLevelResult = await _beatmapLevelsModel.GetBeatmapLevelAsync(selectedLevel.levelID, token.Token);

            callback?.Invoke(!getBeatmapLevelResult.isError, getBeatmapLevelResult.beatmapLevel);
        }

        private static void UploadScore(IDifficultyBeatmap levelDifficulty, LevelCompletionResults results, out bool newHighScore)
        {
            var freePlayCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            var dataModel = freePlayCoordinator.GetPrivateField<PlayerDataModel>("_playerDataModel");

            PlayerData currentLocalPlayer = dataModel.playerData;
            PlayerLevelStatsData playerLevelStatsData = currentLocalPlayer.GetPlayerLevelStatsData(levelDifficulty.level.levelID, levelDifficulty.difficulty, levelDifficulty.parentDifficultyBeatmapSet.beatmapCharacteristic);

            int prevHighScore = playerLevelStatsData.highScore;

            LevelCompletionResults levelCompletionResults = results;
            playerLevelStatsData.UpdateScoreData(levelCompletionResults.modifiedScore, levelCompletionResults.maxCombo, levelCompletionResults.fullCombo, levelCompletionResults.rank);

            newHighScore = playerLevelStatsData.highScore > prevHighScore;

            var platFormLeaderBoardsModel = freePlayCoordinator.GetPrivateField<PlatformLeaderboardsModel>("_platformLeaderboardsModel");
            platFormLeaderBoardsModel.UploadScore(levelDifficulty, results.rawScore, results.modifiedScore, results.fullCombo, results.goodCutsCount, results.badCutsCount, results.missedCount, results.maxCombo, results.gameplayModifiers);//AddScoreFromComletionResults(levelDifficulty, levelCompletionResults);
        }
    }
}