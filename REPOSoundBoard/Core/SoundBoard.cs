using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using REPOSoundBoard.Config;
using REPOSoundBoard.Core.Media;
using REPOSoundBoard.Core.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.Core
{
    public class SoundBoard : MonoBehaviour
    {
        public static SoundBoard Instance;
        
        public List<SoundButton> SoundButtons { get; } = new List<SoundButton>();
        public Hotkey StopHotkey;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        private Recorder _recorder;
        private AudioSource _audioSource;
        private bool _isPlaying;
        private bool _enabled;
        private SoundButton _currentSoundButton;
        private Coroutine _playCoroutine;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            this._audioSource = this.gameObject.AddComponent<AudioSource>();
        }

        public void LoadConfig(SoundBoardConfig config)
        {
            // Enable/disabled
            this._enabled = config.Enabled;
            
            // Stop hotkey
            this.StopHotkey = config.StopHotkey;
            this.StopHotkey.OnPressed(this.StopCurrent);
            REPOSoundBoard.Instance.HotkeyManager.RegisterHotkey(config.StopHotkey);

            // Sound buttons
            foreach (var configSoundButton in config.SoundButtons)
            {
                if (configSoundButton.Hotkey == null || configSoundButton.Path == null)
                {
                    continue;
                }
                
                var sb = SoundButton.FromConfig(configSoundButton);
                this.StartCoroutine(sb.LoadClip());
                this.AddSoundButton(sb);
            }
        }

        public void AddSoundButton(SoundButton soundButton)
        {
            soundButton.Hotkey.OnPressed(() => this.Play(soundButton));

            REPOSoundBoard.Instance.HotkeyManager.RegisterHotkey(soundButton.Hotkey);
            this.SoundButtons.Add(soundButton);
        }

        public void RemoveSoundButton(SoundButton soundButton)
        {
            this.SoundButtons.Remove(soundButton);
            REPOSoundBoard.Instance.HotkeyManager.UnregisterHotkey(soundButton.Hotkey);
        }


        public void ChangeRecorder(Recorder recorder)
        {
            this.StopCurrent();
            this._recorder = recorder;
        }


		public void StartLocalPlayback(SoundButton soundButton, bool startStopCoroutine = true) {

			if (soundButton.Clip == null || soundButton.Clip.State != MediaClip.MediaClipState.Loaded) {
				return;
			}

			this.StopCurrent();

			this._isPlaying = true;
			this._currentSoundButton = soundButton;
			this._audioSource.clip = soundButton.Clip.AudioClip;
			this._audioSource.volume = soundButton.Volume;
			this._audioSource.Play();

			if (startStopCoroutine) {
				this._playCoroutine = this.StartCoroutine(this.StopAfterEnd(soundButton));
			}
		}


        private void Play(SoundButton soundButton)
        {
            if (!this._enabled || !soundButton.Enabled || soundButton.Clip == null || soundButton.Clip.State != MediaClip.MediaClipState.Loaded)
            {
				return;
            }
            
            if (this._recorder == null || this._audioSource == null)
			{
				return;
			}

			if (this._isPlaying)
			{
				this.StopCurrent();
			}

			this._isPlaying = true;
			this._currentSoundButton = soundButton;

			this._recorder.TransmitEnabled = false;
			this._recorder.SourceType = Recorder.InputSourceType.AudioClip;
			this._recorder.AudioClip = soundButton.Clip.AudioClip;
			this._recorder.TransmitEnabled = true;

			// Also play locally through AudioSource
			this.StartLocalPlayback(soundButton, false);

			this._playCoroutine = this.StartCoroutine(this.StopAfterEnd(soundButton));
        }

		private IEnumerator StopAfterEnd(SoundButton soundButton) {

			if (soundButton.Clip == null) {
				yield break;
			}

			yield return new WaitForSeconds(soundButton.Clip.AudioClip.length);

			this.StopCurrent();
		}

        private void StopCurrent()
        {
	        if (!this._isPlaying)
	        {
		        return;
	        }

	        if (this._playCoroutine != null)
            {
                this.StopCoroutine(this._playCoroutine);
                this._playCoroutine = null;
            }
	        
			_audioSource.Stop();

			if (this._recorder != null)
			{
				this._recorder.SourceType = Recorder.InputSourceType.Microphone;
				this._recorder.AudioClip = null;
				this._recorder.TransmitEnabled = true;
			}

            this._isPlaying = false;
            this._currentSoundButton = null;
        }

        private void OnDestroy()
        {
            // Update the config
            var soundBoardConfig = new SoundBoardConfig();
            soundBoardConfig.Enabled = this._enabled;
            soundBoardConfig.StopHotkey = this.StopHotkey;

            foreach (var soundButton in SoundButtons)
            {
                soundBoardConfig.SoundButtons.Add(soundButton.CreateConfig());
            }

            REPOSoundBoard.Instance.Config.SoundBoard = soundBoardConfig;
            REPOSoundBoard.Instance.Config.SaveToFile();
        }
    }
}