using REPOSoundBoard.Config;
using REPOSoundBoard.Core;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI.Components
{
    public class GeneralSettingsUI
    {
        private bool _selectionStopHotkey = false;

        public void Draw()
        {
            SoundBoard.Instance.Enabled = IMGUIUtils.LabeledToggle("Global enabled:", SoundBoard.Instance.Enabled);
            _selectionStopHotkey = IMGUIUtils.LabeledHotkeyInput("Stop hotkey:", SoundBoard.Instance.StopHotkey, _selectionStopHotkey);

            IMGUIUtils.HorizontalGroup(() =>
            {
                if (GUILayout.Button("Save changes", GUILayout.ExpandWidth(true)))
                {
                    // Update the config
                    var soundBoardConfig = new SoundBoardConfig();
                    soundBoardConfig.Enabled = SoundBoard.Instance.Enabled;
                    soundBoardConfig.StopHotkey = SoundBoard.Instance.StopHotkey;

                    foreach (var soundButton in SoundBoard.Instance.SoundButtons)
                    {
                        soundBoardConfig.SoundButtons.Add(soundButton.CreateConfig());
                    }

                    REPOSoundBoard.Instance.Config.SoundBoard = soundBoardConfig;
                    REPOSoundBoard.Instance.Config.SaveToFile();
                }

                if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
                {
                    SoundBoardUI.Instance.Visible = false;
                }
            });
        }
    }
}