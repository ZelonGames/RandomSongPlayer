using System;
using System.Collections.Generic;
using System.Linq;
using IPA.Utilities;
using UnityEngine;
using HMUI;
using Newtonsoft.Json;
using BeatSaberMarkupLanguage;

namespace RandomSongPlayer
{
    internal class RandomSongMenu : FlowCoordinator
    {
        private MainFlowCoordinator mainFlowCoordinator = null;

        private CustomPreviewBeatmapLevel beatmap;
        private BeatmapDifficulty difficulty;
        private IDifficultyBeatmap levelDifficulty;
        private LevelCompletionResults levelCompletionResults;
        private ResultsViewController resultsViewController;
        private bool newHighScore = false;


        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().First();
                resultsViewController.Init(levelCompletionResults, levelDifficulty, false, newHighScore);

                resultsViewController.continueButtonPressedEvent += OnContinueButtonPressed;
                resultsViewController.restartButtonPressedEvent += OnRestartButtonPressed;

                if (addedToHierarchy && resultsViewController != null)
                    ProvideInitialViewControllers(resultsViewController, null, null);
            }
        }


        public void Show(CustomPreviewBeatmapLevel customPreviewBeatmapLevel, BeatmapDifficulty difficulty, IDifficultyBeatmap levelDifficulty, LevelCompletionResults results, bool newHighScore)
        {
            this.beatmap = customPreviewBeatmapLevel;
            this.difficulty = difficulty;
            this.levelDifficulty = levelDifficulty;
            this.levelCompletionResults = results;
            this.newHighScore = newHighScore;

            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            BeatSaberUI.PresentFlowCoordinator(mainFlowCoordinator, this);
        }

        public void Hide()
        {
            resultsViewController.continueButtonPressedEvent -= OnContinueButtonPressed;
            resultsViewController.restartButtonPressedEvent -= OnRestartButtonPressed;

            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            BeatSaberUI.DismissFlowCoordinator(mainFlowCoordinator, this);
        }

        public void OnContinueButtonPressed(ResultsViewController resultsViewController)
        {
            Hide();
        }

        public void OnRestartButtonPressed(ResultsViewController resultsViewController)
        {
            Hide();
            LevelHelper.PlayLevel(beatmap, difficulty);
        }

    }
}