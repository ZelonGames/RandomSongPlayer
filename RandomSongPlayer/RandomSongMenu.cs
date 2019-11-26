using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using UnityEngine;
using UnityEngine.UI;
using CustomUI.UIElements;
using VRUI;

namespace RandomSongPlayer
{
    internal class MainViewController : VRUINavigationController
    {
        public event Action BackButtonClicked;

        private Button backButton;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
                backButton = BeatSaberUI.CreateBackButton(rectTransform, BackButtonClicked.Invoke);
        }
    }

    internal class RandomSongMenu : FlowCoordinator
    {
        private MainFlowCoordinator mainFlowCoordinator = null;

        private CustomPreviewBeatmapLevel beatmap;
        private BeatmapDifficulty difficulty;
        private IDifficultyBeatmap levelDifficulty;
        private LevelCompletionResults levelCompletionResults;
        private ResultsViewController resultsViewController;
        private bool newHighScore = false;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().First();
                resultsViewController.Init(levelCompletionResults, levelDifficulty, newHighScore);

                resultsViewController.continueButtonPressedEvent += OnContinueButtonPressed;
                resultsViewController.restartButtonPressedEvent += OnRestartButtonPressed;
            }

            //var leaderboardViewController = Resources.FindObjectsOfTypeAll<LeaderboardViewController>().First();

            if (activationType == ActivationType.AddedToHierarchy)
                ProvideInitialViewControllers(resultsViewController, null, null);
        }

        public void Show(CustomPreviewBeatmapLevel customPreviewBeatmapLevel, BeatmapDifficulty difficulty, IDifficultyBeatmap levelDifficulty, LevelCompletionResults results, bool newHighScore)
        {
            this.beatmap = customPreviewBeatmapLevel;
            this.difficulty = difficulty;
            this.levelDifficulty = levelDifficulty;
            this.levelCompletionResults = results;
            this.newHighScore = newHighScore;

            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlowCoordinator.PresentFlowCoordinatorOrAskForTutorial(this);
        }

        public void Hide()
        {
            resultsViewController.continueButtonPressedEvent -= OnContinueButtonPressed;
            resultsViewController.restartButtonPressedEvent -= OnRestartButtonPressed;

            mainFlowCoordinator.InvokeMethod("DismissFlowCoordinator", new object[]
            {
                this,
                null,
                false
            });
        }

        public void OnContinueButtonPressed(ResultsViewController resultsViewController)
        {
            Hide();
        }

        public void OnRestartButtonPressed(ResultsViewController resultsViewController)
        {
            LevelHelper.PlayLevel(beatmap, difficulty);
        }
    }
}
