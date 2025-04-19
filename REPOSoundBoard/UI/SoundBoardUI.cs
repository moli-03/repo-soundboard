using System;
using REPOSoundBoard.Patches;
using REPOSoundBoard.UI.Components;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI
{
    public class SoundBoardUI : MonoBehaviour
    {
        public static SoundBoardUI Instance;

        private bool _isVisible = false;
        public bool Visible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                if (_isVisible)
                {
                    UnlockCursor();
                }
                else
                {
                    LockCursor();
                }
            }
        }

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

        private void Update()
        {
            if (Visible)
            {
                InputManager.instance.DisableAiming();
                InputManager.instance.DisableMovement();
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

			_generalSettingsWindow.Draw();
            _soundButtonsWindow.Draw();
        }


        private void UnlockCursor()
        {
            MenuCursorPatch.HideInGameCursor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void LockCursor()
        {
            MenuCursorPatch.HideInGameCursor = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        private void OnDestroy()
        {
            REPOSoundBoard.Instance.SaveConfig();
        }
    }
}