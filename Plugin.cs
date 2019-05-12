using BS_Utils.Gameplay;
using IPA;
using UnityEngine.SceneManagement;
using CustomUI.GameplaySettings;
using UnityEngine;
using IPA.Config;
using System.Reflection;

namespace AutoPause
{
    public class Plugin : IBeatSaberPlugin
    {
        internal static BS_Utils.Utilities.Config AutoPauseConfig = new BS_Utils.Utilities.Config("AutoPause");
        private static Sprite _autoLogo;
        private static Sprite _onoffButton;
        private static Sprite _sensitivty;
        private static Sprite _sensiyellow;
        private static Sprite _sensired;

        public void OnApplicationStart()
        {
        }

        public void OnApplicationQuit()
        {
        }

        public void OnFixedUpdate()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
                AutoPause.Load();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
            {
                Gamemode.Init();
                CreateUI();
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        //UI

        public void CreateUI()
        {
            GetIcons();

            GameplaySettingsUI.CreateSubmenuOption(GameplaySettingsPanels.ModifiersLeft, "AutoPause", "MainMenu", "apMenu", "Settings for AutoPause", _autoLogo);
            var apEnabled = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "Enabled", "apMenu", "Activate/Deactivate AutoPause", _onoffButton);
            var apWhite = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "Low Sensitivity", "apMenu", "AutoPause will trigger for very big issues.", _sensitivty);
            var apYellow = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "SensiMain", "apMenu", "AutoPause will work normally.", _sensiyellow);
            var apRed = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.ModifiersLeft, "SensiRed", "apMenu", "AutoPause is on full alert.", _sensired);
            apWhite.AddConflict("SensiMain");
            apWhite.AddConflict("SensiRed");
            apYellow.AddConflict("Low Sensitivity");
            apYellow.AddConflict("SensiRed");
            apRed.AddConflict("Low Sensitivity");
            apRed.AddConflict("SensiMain");

            apEnabled.GetValue = AutoPauseConfig.GetBool("Main", "Enabled", true, true);
            apWhite.GetValue = AutoPauseConfig.GetBool("Main", "LowSense", false, true);
            apYellow.GetValue = AutoPauseConfig.GetBool("Main", "SensiMain", true, true);
            apRed.GetValue = AutoPauseConfig.GetBool("Main", "SensiRed", false, true);

            apEnabled.OnToggle += (value) => { AutoPauseConfig.SetBool("Main", "Enabled", value); };
            apWhite.OnToggle += (value) => { AutoPauseConfig.SetBool("Main", "LowSense", value); };
            apYellow.OnToggle += (value) => { AutoPauseConfig.SetBool("Main", "SensiMain", value); };
            apRed.OnToggle += (value) => { AutoPauseConfig.SetBool("Main", "SensiRed", value); };
        }

        internal void GetIcons()
        {
            if (_autoLogo == null)
                _autoLogo = CustomUI.Utilities.UIUtilities.LoadSpriteFromResources("AutoPause.Utilities.AutoPause.png");

            if (_onoffButton == null)
                _onoffButton = CustomUI.Utilities.UIUtilities.LoadSpriteFromResources("AutoPause.Utilities.Power.png");

            if (_sensitivty == null)
                _sensitivty = CustomUI.Utilities.UIUtilities.LoadSpriteFromResources("AutoPause.Utilities.sensitivity.png");

            if (_sensiyellow == null)
                _sensiyellow = CustomUI.Utilities.UIUtilities.LoadSpriteFromResources("AutoPause.Utilities.sensiyellow.png");

            if (_sensired == null)
                _sensired = CustomUI.Utilities.UIUtilities.LoadSpriteFromResources("AutoPause.Utilities.sensired.png");
        } 
    }
}
