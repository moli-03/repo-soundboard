using REPOSoundBoard.Patches;
using REPOSoundBoard.UI.Components;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI
{
    public class SoundBoardUI : MonoBehaviour
    {
        public static SoundBoardUI Instance;
        public bool Visible = false;

        private const float DefaultWindowGap = 20f;
        
        private IMGUIWindow _generalSettingsWindow;
        private GeneralSettingsUI _generalSettingsUI;
        private const float GeneralSettingsWindowWidth = 340f;
        private const float GeneralSettingsWindowHeight = 60f;
        
        private IMGUIWindow _soundButtonsWindow;
        private SoundButtonsUI _soundButtonsUI;
        private const float SoundButtonsWindowWidth = 600f;
        private const float SoundButtonsWindowHeight = 650f;
        
        
        public void Start()
        {
            Instance = this;

            float totalWidth = GeneralSettingsWindowWidth + SoundButtonsWindowWidth + DefaultWindowGap;
            float totalHeight = Mathf.Max(GeneralSettingsWindowHeight, SoundButtonsWindowHeight);
            
            this._generalSettingsUI = new GeneralSettingsUI();
            this._generalSettingsWindow = new IMGUIWindow(
                "General Settings",
                new Rect(( Screen.width - totalWidth) / 2f, (Screen.height - totalHeight) / 2f, GeneralSettingsWindowWidth, GeneralSettingsWindowHeight),
                windowId => _generalSettingsUI.Draw()
            );
            
            this._soundButtonsUI = new SoundButtonsUI();
            this._soundButtonsWindow = new IMGUIWindow(
                "Sound Buttons",
                new Rect((Screen.width - totalWidth) / 2f + GeneralSettingsWindowWidth + DefaultWindowGap, (Screen.height - totalHeight) / 2f, SoundButtonsWindowWidth, SoundButtonsWindowHeight),
                windowId => _soundButtonsUI.Draw()
            );
        }

        private void OnGUI()
        {
            MenuCursorPatch.HideInGameCursor = Visible;
            
            if (!Visible) return;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

			_generalSettingsWindow.Draw();
            _soundButtonsWindow.Draw();
        }
    }
}