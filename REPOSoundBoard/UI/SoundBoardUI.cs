using System;
using REPOSoundBoard.Core;
using REPOSoundBoard.Patches;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI
{
    public class SoundBoardUI : MonoBehaviour
    {
        public static SoundBoardUI Instance;
        public static bool Visible = true;

        private const float DefaultWindowGap = 20f;
        
        private IMGUIWindow _generalSettingsWindow;
        private const float GeneralSettingsWindowWidth = 340f;
        private const float GeneralSettingsWindowHeight = 60f;
        private bool _selectionStopHotkey = false;
        
        private IMGUIWindow _soundButtonsWindow;
        private const float SoundButtonsWindowWidth = 600f;
        private const float SoundButtonsWindowHeight = 650f;
        
        
        public void Start()
        {
            Instance = this;

            float totalWidth = GeneralSettingsWindowWidth + SoundButtonsWindowWidth + DefaultWindowGap;
            float totalHeight = Mathf.Max(GeneralSettingsWindowHeight, SoundButtonsWindowHeight);
            
            this._generalSettingsWindow = new IMGUIWindow(
                "General Settings",
                new Rect(( Screen.width - totalWidth) / 2f, (Screen.height - totalHeight) / 2f, GeneralSettingsWindowWidth, GeneralSettingsWindowHeight),
                windowId =>
                {
                    SoundBoard.Instance.Enabled = IMGUIUtils.LabeledToggle("Global enabled:", SoundBoard.Instance.Enabled);
                    _selectionStopHotkey = IMGUIUtils.LabeledHotkeyInput("Stop hotkey:", SoundBoard.Instance.StopHotkey, _selectionStopHotkey);
                }
            );
            
            this._soundButtonsWindow = new IMGUIWindow(
                "Sound Buttons",
                new Rect((Screen.width - totalWidth) / 2f + GeneralSettingsWindowWidth + DefaultWindowGap, (Screen.height - totalHeight) / 2f, SoundButtonsWindowWidth, SoundButtonsWindowHeight),
                windowId =>
                {
                    SoundBoard.Instance.Enabled = IMGUIUtils.LabeledToggle("Global enabled:", SoundBoard.Instance.Enabled);
                    _selectionStopHotkey = IMGUIUtils.LabeledHotkeyInput("Stop hotkey:", SoundBoard.Instance.StopHotkey, _selectionStopHotkey);
                }
            );
        }

        private void OnGUI()
        {
            MenuCursorPatch.HideInGameCursor = Visible;
            
            if (!Visible) return;
            
            Cursor.lockState = CursorLockMode.None;
            
			_generalSettingsWindow.Draw();
            _soundButtonsWindow.Draw();
        }
    }
}