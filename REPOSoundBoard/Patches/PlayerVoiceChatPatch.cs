﻿using HarmonyLib;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;

namespace REPOSoundBoard.Patches
{
    public class PlayerVoiceChatPatch
    {
        [HarmonyPatch(typeof(PlayerVoiceChat), "Start")]
        [HarmonyPostfix]
        public static void PostStart(PlayerVoiceChat __instance, ref Recorder ___recorder, ref PhotonView ___photonView)
        {
            if (!___photonView.IsMine)
            {
                return;
            }
            
            REPOSoundBoard.Instance.SoundBoard.ChangeRecorder(___recorder);
        }
    }
}