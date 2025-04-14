using System;
using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using REPOSoundBoard.Core;
using REPOSoundBoard.Core.Media;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI.Components
{
    public class SoundButtonUI
    {
        private static GUIStyle _pathStyle;
        private static GUIStyle PathStyle
        {
            get
            {
                if (_pathStyle == null)
                {
                    _pathStyle = new GUIStyle(GUI.skin.label);
                    _pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                    _pathStyle.fontStyle = FontStyle.Italic;
                    _pathStyle.fontSize = 12;
                }
                    
                return _pathStyle;
            }
        }
        
        private static GUIStyle _disabledButtonLabelStyle;
        private static GUIStyle DisabledButtonLabelStyle
        {
            get
            {
                if (_disabledButtonLabelStyle == null)
                {
                    _disabledButtonLabelStyle = new GUIStyle(GUI.skin.label);
                    _disabledButtonLabelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                }
                
                return _disabledButtonLabelStyle;
            }
        }
        
        public SoundButton SoundButton { get; }
        private bool _isEditing = false;
        private bool _isEditingHotkey = false;
        private bool _changingPath = false;
        private string _pathInput;

        [CanBeNull] public event Action<SoundButtonUI> OnDeleteClicked;

        public SoundButtonUI(SoundButton soundButton, bool editing = false)
        {
            SoundButton = soundButton;
            _isEditing = editing;
            _pathInput = SoundButton.Clip?.OriginalPath ?? string.Empty;
        }

        public void Draw()
        {
            IMGUIUtils.BoxGroup(() =>
            {
                this.DrawState();
                
                if (!_isEditing)
                {
                    DrawDetailUI();
                }
                else
                {
                    DrawEditUI();
                }
            });
        }

		private static Color GetStateColor(MediaClip.MediaClipState state)
        {
            return state switch
            {
                MediaClip.MediaClipState.Idle => Color.white,
                MediaClip.MediaClipState.Converting => Color.yellow,
                MediaClip.MediaClipState.Converted => Color.white,
                MediaClip.MediaClipState.FailedToConvert => Color.red,
                MediaClip.MediaClipState.Loading => Color.cyan,
                MediaClip.MediaClipState.Loaded => Color.green,
                MediaClip.MediaClipState.FailedToLoad => Color.red,
                _ => Color.white
            };
        }

        private void DrawState()
        {
            if (SoundButton.Clip == null || SoundButton.Clip.State == MediaClip.MediaClipState.Loaded)
            {
                return;
            }
            
            IMGUIUtils.HorizontalGroup(() =>
            {
                GUILayout.Label("State:", GUILayout.ExpandWidth(false));
                var style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = GetStateColor(SoundButton.Clip.State);
                GUILayout.Label(SoundButton.Clip.StateMessage, style, GUILayout.ExpandWidth(true));
            });
        }

        private void DrawDetailUI()
        {
            IMGUIUtils.HorizontalGroup(() =>
            {
                // Name of the button

                if (SoundButton.Enabled)
                {
                    GUILayout.Label(SoundButton.Name, GUILayout.ExpandWidth(true));
                }
                else
                {
                    GUILayout.Label(SoundButton.Name + " [disabled]", DisabledButtonLabelStyle, GUILayout.ExpandWidth(true));
                }

                // Hotkey
                if (SoundButton.Hotkey.Keys.Count == 0)
                {
                    GUILayout.Button("[no hotkey]", GUILayout.ExpandWidth(false));
                }
                else
                {
                    string keys = string.Join(" + ", SoundButton.Hotkey.Keys);
                    if (GUILayout.Button(keys, GUILayout.ExpandWidth(false)))
                    {
                        SoundBoard.Instance.Play(SoundButton, true);
                    }
                }
                
                // Edit
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    _isEditing = true;
                }
                
                // Delete
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    OnDeleteClicked?.Invoke(this);
                }
            });
            
            // Path
            if (SoundButton.Clip == null)
            {
                GUILayout.Label("No file selected...", PathStyle, GUILayout.ExpandWidth(true));
            }
            else
            {
                GUILayout.Label(SoundButton.Clip.OriginalPath, PathStyle, GUILayout.ExpandWidth(true));
            }
        }

        private void DrawEditUI()
        {
            
            IMGUIUtils.VerticalGroup(() =>
            {
                // Name
                this.SoundButton.Name = IMGUIUtils.LabeledTextField("Name:", SoundButton.Name);
                
                // Hotkey
                this._isEditingHotkey = IMGUIUtils.LabeledHotkeyInput("Hotkey:", this.SoundButton.Hotkey, this._isEditingHotkey);
                
                // Enabled
                this.SoundButton.Enabled = IMGUIUtils.LabeledToggle("Enabled:", this.SoundButton.Enabled);
                
                // Volume
                this.SoundButton.Volume = IMGUIUtils.LabeledSlider("Volume:", this.SoundButton.Volume, 0f, 1f);
                
                // Path
                this._pathInput = IMGUIUtils.LabeledPathInput("Path:", SoundButton.Clip?.OriginalPath ?? string.Empty, _pathInput, ref _changingPath, OnPathChanged);

                if (GUILayout.Button("Done", GUILayout.ExpandWidth(true)))
                {
                    this._isEditing = false;
                }
            });
        }


        private void OnPathChanged(string path)
        {
            SoundButton.Clip = new MediaClip(path);
            SoundBoard.Instance.StartCoroutine(SoundButton.LoadClip());
        }
    }
}