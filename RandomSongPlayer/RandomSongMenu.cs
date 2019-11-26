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
        private MainViewController mainViewController;

        private MainFlowCoordinator mainFlowCoordinator = null;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (!firstActivation || activationType != ActivationType.AddedToHierarchy)
                return;

            title = "Random Song Player";

            mainViewController = BeatSaberUI.CreateViewController<MainViewController>();
            mainViewController.BackButtonClicked += Hide;

            var playerSettingsViewController = BeatSaberUI.CreateViewController<PlayerSettingsViewController>();

            ProvideInitialViewControllers(mainViewController, null, null, null);
            SetViewControllersToNavigationConctroller(mainViewController, new VRUIViewController[] { });

            BeatSaberUI.CreateText(mainViewController.rectTransform, "hello world!", new Vector2(2, 2));            
        }

        public void Show()
        {
            mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlowCoordinator.PresentFlowCoordinatorOrAskForTutorial(this);
        }

        public void Hide()
        {
            mainFlowCoordinator.InvokeMethod("DismissFlowCoordinator", new object[]
            {
                this,
                null,
                false
            });
        }
    }
}
