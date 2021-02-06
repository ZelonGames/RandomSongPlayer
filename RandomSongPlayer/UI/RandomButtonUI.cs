using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace RandomSongPlayer.UI
{
    public class RandomButtonUI : NotifiableSingleton<RandomButtonUI>
    {
        // Plugin is RandomSongPlayer, need this value to download the map
        private Plugin plugin;
        private LevelSelectionNavigationController levelCollection;

        [UIComponent("random-button")]
        internal UnityEngine.UI.Button button;

        internal void Setup(Plugin parentPlugin)
        {
            plugin = parentPlugin;
            levelCollection = Resources.FindObjectsOfTypeAll<LevelSelectionNavigationController>().First();
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "RandomSongPlayer.UI.RandomButton.bsml"), levelCollection.gameObject, this);
        }

        public void Show()
        {
            button.gameObject.SetActive(true);
        }

        public void Hide()
        {
            button.gameObject.SetActive(false);
        }

        [UIAction("button-click")]
        async internal void RandomLevelButtonClick()
        {
            button.interactable = false;
            await plugin.SelectRandomSongAsync();
            button.interactable = true;
        }
    }
}
