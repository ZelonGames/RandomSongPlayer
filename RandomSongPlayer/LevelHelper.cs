using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomSongPlayer
{
    internal static class LevelHelper
    {
        public delegate void LevelClearedEventHandler(LevelCompletionResults results);
        public static event LevelClearedEventHandler LevelCleared = null;

        internal static void StartLevel(IBeatmapLevel level, BeatmapCharacteristicSO characteristics, BeatmapDifficulty difficulty)
        {
            MenuTransitionsHelperSO menuSceneSetupData = Resources.FindObjectsOfTypeAll<MenuTransitionsHelperSO>().FirstOrDefault();
            PlayerData playerSettings = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().FirstOrDefault().playerData;

            var gamePlayModifiers = new GameplayModifiers();
            gamePlayModifiers.IsWithoutModifiers();

            IDifficultyBeatmap levelDifficulty = BeatmapLevelDataExtensions.GetDifficultyBeatmap(level.beatmapLevelData, characteristics, difficulty);

            menuSceneSetupData.StartStandardLevel(levelDifficulty,
                playerSettings.overrideEnvironmentSettings.overrideEnvironments ? playerSettings.overrideEnvironmentSettings : null,
                playerSettings.colorSchemesSettings.overrideDefaultColors ? playerSettings.colorSchemesSettings.GetSelectedColorScheme() : null,
                gamePlayModifiers,
                playerSettings.playerSpecificSettings,
                null, "Exit", playerSettings.playerSpecificSettings.disableSFX, () => { }, (StandardLevelScenesTransitionSetupDataSO sceneTransition, LevelCompletionResults results) =>
                {

                    switch (results.levelEndStateType)
                    {
                        case LevelCompletionResults.LevelEndStateType.None:
                            break;
                        case LevelCompletionResults.LevelEndStateType.Cleared:
                            UploadScore(levelDifficulty, results);
                            LevelCleared?.Invoke(results);
                            break;
                        case LevelCompletionResults.LevelEndStateType.Failed:
                            break;
                        default:
                            break;
                    }

                });
        }

        internal static async void LoadBeatmapLevelAsync(IPreviewBeatmapLevel selectedLevel, Action<AdditionalContentModelSO.EntitlementStatus, bool, IBeatmapLevel> callback)
        {
            var token = new CancellationTokenSource();

            var _contentModelSO = Resources.FindObjectsOfTypeAll<AdditionalContentModelSO>().FirstOrDefault();
            var _beatmapLevelsModel = Resources.FindObjectsOfTypeAll<BeatmapLevelsModelSO>().FirstOrDefault();

            var entitlementStatus = await _contentModelSO.GetLevelEntitlementStatusAsync(selectedLevel.levelID, token.Token);

            if (entitlementStatus == AdditionalContentModelSO.EntitlementStatus.Owned)
            {
                BeatmapLevelsModelSO.GetBeatmapLevelResult getBeatmapLevelResult = await _beatmapLevelsModel.GetBeatmapLevelAsync(selectedLevel.levelID, token.Token);

                callback?.Invoke(entitlementStatus, !getBeatmapLevelResult.isError, getBeatmapLevelResult.beatmapLevel);
            }
            else
                callback?.Invoke(entitlementStatus, false, null);
        }

        private static void UploadScore(IDifficultyBeatmap levelDifficulty, LevelCompletionResults results)
        {
            var freePlayCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            var dataModel = freePlayCoordinator.GetPrivateField<PlayerDataModelSO>("_playerDataModel");

            PlayerData currentLocalPlayer = dataModel.playerData;
            PlayerLevelStatsData playerLevelStatsData = currentLocalPlayer.GetPlayerLevelStatsData(levelDifficulty.level.levelID, levelDifficulty.difficulty, levelDifficulty.parentDifficultyBeatmapSet.beatmapCharacteristic);
            LevelCompletionResults levelCompletionResults = results;
            playerLevelStatsData.UpdateScoreData(levelCompletionResults.modifiedScore, levelCompletionResults.maxCombo, levelCompletionResults.fullCombo, levelCompletionResults.rank);
            var platFormLeaderBoardsModel = freePlayCoordinator.GetPrivateField<PlatformLeaderboardsModel>("_platformLeaderboardsModel");
            platFormLeaderBoardsModel.AddScoreFromComletionResults(levelDifficulty, levelCompletionResults);
        }
    }
}
