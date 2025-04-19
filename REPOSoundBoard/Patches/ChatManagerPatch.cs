using HarmonyLib;
using REPOSoundBoard.UI;

namespace REPOSoundBoard.Patches
{
    public static class ChatManagerPatch
    {
        [HarmonyPatch(typeof(ChatManager), "StateInactive")]
        [HarmonyPrefix]
        public static bool BeforeStateInactive(ref string ___chatMessage, ref bool ___chatActive)
        {
            if (SoundBoardUI.Instance.Visible)
            {
                ChatUI.instance.Hide();
                ___chatMessage = "";
                ___chatActive = false;
                return false;
            }

            return true;
        }
    }
}