using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using REPOSoundBoard.Config;
using UnityEngine;

namespace REPOSoundBoard.Sound
{
    public class SoundBoard : MonoBehaviour
    {
        private List<SoundButton> _soundButtons = new List<SoundButton>();
        
        private Recorder _recorder;
        private AudioSource _audioSource;
        private bool _isPlaying;
        private SoundButton _currentSoundButton;

        public void LoadConfig(SoundBoardConfig config)
        {
            config.StopHotkey.OnPressed(this.StopCurrent);
            REPOSoundBoard.Instance.HotkeyManager.RegisterHotkey(config.StopHotkey);
            
            foreach (var configSoundButton in config.SoundButtons)
            {
                if (configSoundButton.Hotkey == null || configSoundButton.Path == null)
                {
                    continue;
                }
                
                var soundClip = new SoundClip(configSoundButton.Path);
                var soundButton = new SoundButton(soundClip, configSoundButton.Hotkey, configSoundButton.Volume);
                
                this.AddSoundButton(soundButton);
            }
        }

        public void AddSoundButton(SoundButton soundButton)
        {
            soundButton.Hotkey.OnPressed(() =>
            {
                this.StartCoroutine(this.Play(soundButton));
            });
           
            REPOSoundBoard.Instance.HotkeyManager.RegisterHotkey(soundButton.Hotkey);
            this._soundButtons.Add(soundButton);
        }

        public void RemoveSoundButton(SoundButton soundButton)
        {
            this._soundButtons.Remove(soundButton);
            REPOSoundBoard.Instance.HotkeyManager.UnregisterHotkey(soundButton.Hotkey);
        }


        public void ChangeRecorder(Recorder recorder)
        {
            this.StopCurrent();
            
            this._recorder = recorder;
        }

        public void ChangeAudioSource(AudioSource source)
        {
            this.StopCurrent();
            
            this._audioSource = source;
        }

        private IEnumerator Play(SoundButton soundButton)
        {
            if (this._recorder != null && this._audioSource != null && soundButton.SoundClip.IsLoaded)
            {
                if (this._isPlaying)
                {
                    this.StopCurrent();
                }
                
                this._isPlaying = true;
                this._currentSoundButton = soundButton;
                
                this._recorder.TransmitEnabled = false;
                this._recorder.SourceType = Recorder.InputSourceType.AudioClip;
                this._recorder.AudioClip = soundButton.SoundClip.AudioClip;
                this._recorder.TransmitEnabled = true;
                
                // Also play locally through AudioSource
                this._audioSource.clip = soundButton.SoundClip.AudioClip;
                this._audioSource.volume = soundButton.Volume;
                this._audioSource.Play();
                
                yield return new WaitForSeconds(soundButton.SoundClip.AudioClip.length);
                
                this.Stop(soundButton);
            }
        }

        private void StopCurrent()
        {
            this.Stop(this._currentSoundButton);
        }

        private void Stop(SoundButton soundButton)
        {
            if (this._recorder == null)
            {
                return;
            }
            
            if (!this._isPlaying || this._currentSoundButton != soundButton)
            {
                return;
            }
            
            this._recorder.SourceType = Recorder.InputSourceType.Microphone;
            this._recorder.AudioClip = null;
            this._recorder.TransmitEnabled = true;
            
            if (_audioSource != null)
            {
                _audioSource.Stop();
            }
            
            this._isPlaying = false;
            this._currentSoundButton = null;
        }
    }
}