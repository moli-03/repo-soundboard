using REPOSoundBoard.Core;
using REPOSoundBoard.UI.Utils;

namespace REPOSoundBoard.UI.Components
{
    public class GeneralSettingsUI
    {
        private bool _selectionStopHotkey = false;

        public void Draw()
        {
            SoundBoard.Instance.Enabled = IMGUIUtils.LabeledToggle("Global enabled:", SoundBoard.Instance.Enabled);
            _selectionStopHotkey = IMGUIUtils.LabeledHotkeyInput("Stop hotkey:", SoundBoard.Instance.StopHotkey, _selectionStopHotkey); 
        }
    }
}