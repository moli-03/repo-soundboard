using HarmonyLib;
using UnityEngine;

namespace REPOSoundBoard.Patches
{
    public class MenuCursorPatch
    {
        private static GameObject _buttonMesh;
        public static bool HideInGameCursor = false;

        [HarmonyPatch(typeof(CursorManager), "Update")]
        [HarmonyPrefix]
        public static bool BeforeCursorManagerUpdate()
        {
            return !HideInGameCursor;
        }
        
        [HarmonyPatch(typeof(CursorManager), "Unlock")]
        [HarmonyPrefix]
        public static bool BeforeCursorManagerUnlock()
        {
            return !HideInGameCursor;
        }
    }
}