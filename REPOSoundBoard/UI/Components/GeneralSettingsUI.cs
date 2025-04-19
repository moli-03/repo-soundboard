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
                    REPOSoundBoard.Instance.SaveConfig();
                }

                if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
                {
                    SoundBoardUI.Instance.Visible = false;
                }
            });
        }
    }
}