using System;
using System.Collections.Generic;
using System.Linq;
using REPOSoundBoard.Core;
using REPOSoundBoard.Core.Media;
using UnityEngine;

namespace REPOSoundBoard.UI
{
    public class ModUI : MonoBehaviour
    {
        public static ModUI Instance { get; private set; }
        private bool _isOpen = true;
        private Vector2 _scrollPosition = Vector2.zero;
        private string _searchFilter = string.Empty;

        // UI Configuration
        private const float SOUND_BUTTON_SPACING = 10;
        private const float PADDING_X = 10;
        private const float PADDING_Y = 10;
        private const float SOUND_BUTTON_LINE_SPACING = 5;

        // Button widths
        private const float CHANGE_BUTTON_WIDTH = 60;
        private const float APPLY_BUTTON_WIDTH = 60;
        private const float RESET_BUTTON_WIDTH = 60;
        private const float ADD_BUTTON_WIDTH = 60;
        private const float DELETE_BUTTON_WIDTH = 60;
        private const float CLOSE_BUTTON_WIDTH = 60;

        // Label widths
        private const float NAME_LABEL_WIDTH = 80;
        private const float STATUS_LABEL_WIDTH = 80;
        private const float HOTKEY_LABEL_WIDTH = 80;
        private const float PATH_LABEL_WIDTH = 80;

        // Hotkey binding state
        private bool _isListeningForInput = false;
        private SoundButton _bindingHotkeyForButton = null;
        private List<KeyCode> _currentlyBindingKeys = new List<KeyCode>();
        private List<KeyCode> _possibleKeys;

        // Path selection state
        private string _tmpSelectedPath = null;
        private SoundButton _changingPathForButton = null;

        // Keys that are not allowed as hotkeys
        private readonly List<KeyCode> _disallowedKeys = new List<KeyCode>
        {
            KeyCode.Mouse0, // LMB
            KeyCode.Mouse1, // RMB
        };

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _possibleKeys = new List<KeyCode>((KeyCode[])Enum.GetValues(typeof(KeyCode)));
            _possibleKeys = _possibleKeys.Where(key => !_disallowedKeys.Contains(key)).ToList();
        }

        private void Update()
        {
            if (!_isListeningForInput)
            {
                return;
            }

            DetectKeyPresses();
        }

        private void DetectKeyPresses()
        {
            _currentlyBindingKeys.Clear();
            foreach (var key in _possibleKeys)
            {
                if (Input.GetKey(key))
                {
                    _currentlyBindingKeys.Add(key);
                }
            }
        }

        private void OnGUI()
        {
            if (!_isOpen)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            
            int windowWidth = 800;
            int windowHeight = 500;
            int windowX = (Screen.width - windowWidth) / 2;
            int windowY = (Screen.height - windowHeight) / 2;

            // Create a window
            GUI.Box(new Rect(windowX, windowY, windowWidth, windowHeight), "SoundBoard Settings");

			// Search field
			GUI.Label(new Rect(windowX + 10, windowY + 30, 60, 20), "Search:");
			float searchWidth = windowWidth - 95 - ADD_BUTTON_WIDTH;
			_searchFilter = GUI.TextField(new Rect(windowX + 75, windowY + 30, searchWidth, 20), _searchFilter);

			if (GUI.Button(new Rect(windowX + windowWidth - ADD_BUTTON_WIDTH - 10, windowY + 30, ADD_BUTTON_WIDTH, 20), "Add"))
            {
                SoundBoard.Instance.SoundButtons.Insert(0, new SoundButton());
            }

            // Enable/Disable toggle
            bool enabled = GUI.Toggle(
                new Rect(windowX + 10, windowY + 50, 90, 20),
                SoundBoard.Instance.Enabled,
                "SoundBoard enabled"
            );
            if (enabled != SoundBoard.Instance.Enabled)
            {
                SoundBoard.Instance.Enabled = enabled;
            }

            // Create a scrollable area for sound buttons
            Rect viewportRect = new Rect(windowX + 10, windowY + 80, windowWidth - 20, windowHeight - 90);

            // Calculate content height based on buttons and their dynamic heights
            float contentHeight = CalculateContentHeight(windowWidth);
            Rect contentRect = new Rect(0, 0, windowWidth - 40, contentHeight);

            _scrollPosition = GUI.BeginScrollView(viewportRect, _scrollPosition, contentRect);

            // Display sound buttons
            DrawSoundButtons(windowWidth);

            GUI.EndScrollView();
        }

        private float CalculateContentHeight(int windowWidth)
        {
            float contentHeight = 0;
            foreach (var button in SoundBoard.Instance.SoundButtons)
            {
                if (string.IsNullOrEmpty(_searchFilter) ||
                    button.Name.ToLower().Contains(_searchFilter.ToLower()))
                {
                    float buttonHeight = CalculateSoundButtonHeight(button, windowWidth - 60);
                    contentHeight += buttonHeight + SOUND_BUTTON_SPACING;
                }
            }

            return contentHeight;
        }

        private float CalculateSoundButtonHeight(SoundButton button, float width)
        {
            float nameHeight = 20;
            float statusHeight = 20;
            float controlsHeight = 20;  // Combined height for enable/volume controls
            
            // Calculate hotkey height
            float hotkeyContentWidth = width - PADDING_X * 2 - HOTKEY_LABEL_WIDTH - CHANGE_BUTTON_WIDTH;
            float hotkeyTextHeight =
                GUI.skin.label.CalcHeight(new GUIContent(button.Hotkey.ConcatKeys()), hotkeyContentWidth);
            float hotkeyHeight = Math.Max(20, hotkeyTextHeight);

            // Calculate path height
            float pathContentWidth = width - PADDING_X * 2 - PATH_LABEL_WIDTH - CHANGE_BUTTON_WIDTH;
            float pathTextHeight = 
                GUI.skin.label.CalcHeight(new GUIContent(button.Clip?.OriginalPath ?? ""), pathContentWidth);
            float pathHeight = Math.Max(20, pathTextHeight);

            return PADDING_Y * 2 + SOUND_BUTTON_LINE_SPACING * 4 + nameHeight + statusHeight + hotkeyHeight + pathHeight + controlsHeight;
        }

        private void DrawSoundButtons(int windowWidth)
        {
            float currentY = 0;
            foreach (var soundButton in SoundBoard.Instance.SoundButtons)
            {
                // Skip if doesn't match search filter
                if (!string.IsNullOrEmpty(_searchFilter) &&
                    !soundButton.Name.ToLower().Contains(_searchFilter.ToLower()))
                {
                    continue;
                }

                float buttonWidth = windowWidth - 60;
                float buttonHeight = CalculateSoundButtonHeight(soundButton, buttonWidth);

                // Button background
                GUI.Box(new Rect(10, currentY, buttonWidth, buttonHeight), "");

                float buttonBoxX = 10 + PADDING_X;
                float buttonBoxMaxX = buttonWidth - PADDING_X;
                float buttonBoxY = currentY + PADDING_Y;
                float offsetY = 0;

                // Status
                GUI.Label(new Rect(buttonBoxX, buttonBoxY + offsetY, STATUS_LABEL_WIDTH, 20), "Status:");
                
                string statusText = soundButton.Clip?.StateMessage ?? "No file specified.";
                GUIStyle statusStyle = new GUIStyle(GUI.skin.label);

				if (soundButton.Clip == null) {
					statusStyle.normal.textColor = Color.gray;
				}
                else if (soundButton.Clip.State == MediaClip.MediaClipState.Idle)
                {
                    statusStyle.normal.textColor = Color.gray;
                }
                else if (soundButton.Clip.State == MediaClip.MediaClipState.Converting)
                {
                    statusStyle.normal.textColor = new Color(255, 165, 0);
                }
                else if (soundButton.Clip.State == MediaClip.MediaClipState.Loading)
                {
                    statusStyle.normal.textColor = new Color(255, 165, 0);
                }
                else if (soundButton.Clip.State == MediaClip.MediaClipState.Loaded)
                {
                    statusStyle.normal.textColor = Color.green;
                }
                else if (soundButton.Clip.State == MediaClip.MediaClipState.FailedToConvert)
                {
                    statusStyle.normal.textColor = Color.red;
                }
                else if (soundButton.Clip.State == MediaClip.MediaClipState.FailedToLoad)
                {
                    statusStyle.normal.textColor = Color.red;
                }

                float statusWidth = buttonBoxMaxX - buttonBoxX - STATUS_LABEL_WIDTH;
                GUI.Label(new Rect(buttonBoxX + STATUS_LABEL_WIDTH, buttonBoxY + offsetY, statusWidth, 20), statusText, statusStyle);
                offsetY += 20 + SOUND_BUTTON_LINE_SPACING;

                // Sound name label and field
                GUI.Label(new Rect(buttonBoxX, buttonBoxY + offsetY, NAME_LABEL_WIDTH, 20), "Name:");

                float labelInputWidth = buttonBoxMaxX - buttonBoxX - NAME_LABEL_WIDTH - DELETE_BUTTON_WIDTH;
                soundButton.Name = GUI.TextField(
                    new Rect(buttonBoxX + NAME_LABEL_WIDTH, buttonBoxY + offsetY, labelInputWidth, 20),
                    soundButton.Name
                );
                if (GUI.Button(new Rect(buttonBoxMaxX - DELETE_BUTTON_WIDTH, buttonBoxY + offsetY, ADD_BUTTON_WIDTH, 20), "Delete"))
                {
                    SoundBoard.Instance.SoundButtons.Remove(soundButton);
                }
                offsetY += 20 + SOUND_BUTTON_LINE_SPACING;

                // Volume and Enable controls
                GUI.Label(new Rect(buttonBoxX, buttonBoxY + offsetY, NAME_LABEL_WIDTH, 20), "Volume:");
                soundButton.Volume = GUI.HorizontalSlider(
                    new Rect(buttonBoxX + NAME_LABEL_WIDTH, buttonBoxY + offsetY + 4, 150, 20),
                    soundButton.Volume,
                    0f,
                    1f
                );
                
                GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
                toggleStyle.margin.left = 10;  // Add spacing between checkbox and label
                soundButton.Enabled = GUI.Toggle(
                    new Rect(buttonBoxX + NAME_LABEL_WIDTH + 170, buttonBoxY + offsetY, 70, 20),
                    soundButton.Enabled,
                    "Enabled",
                    toggleStyle
                );
                offsetY += 20 + SOUND_BUTTON_LINE_SPACING;

                // Hotkey section
                GUI.Label(new Rect(buttonBoxX, buttonBoxY + offsetY, HOTKEY_LABEL_WIDTH, 20), "Hotkey:");

                // Regular (not selected)
                if (_bindingHotkeyForButton != soundButton)
                {
                    offsetY += DrawNormalHotkeyUI(soundButton, buttonBoxX, buttonBoxY, offsetY, buttonBoxMaxX);
                }
                // Selected for changing hotkey
                else
                {
                    offsetY += DrawHotkeyBindingUI(soundButton, buttonBoxX, buttonBoxY, offsetY, buttonBoxMaxX);
                }
                
                offsetY += SOUND_BUTTON_LINE_SPACING;

                // Path section
                GUI.Label(new Rect(buttonBoxX, buttonBoxY + offsetY, PATH_LABEL_WIDTH, 20), "Path:");

                if (_changingPathForButton != soundButton)
                {
                    offsetY += DrawNormalPathUI(soundButton, buttonBoxX, buttonBoxY, offsetY, buttonBoxMaxX);
                }
                // Selected for changing path
                else
                {
                    offsetY += DrawPathBindingUI(soundButton, buttonBoxX, buttonBoxY, offsetY, buttonBoxMaxX);
                }

                currentY += buttonHeight + SOUND_BUTTON_SPACING;
            }
        }

        private float DrawNormalPathUI(SoundButton soundButton, float buttonBoxX, float buttonBoxY, float offsetY,
            float buttonBoxMaxX)
        {
            // Display set path
            float width = buttonBoxMaxX - buttonBoxX - CHANGE_BUTTON_WIDTH - PATH_LABEL_WIDTH;
            float height = GUI.skin.label.CalcHeight(new GUIContent(soundButton.Clip?.OriginalPath ?? ""), width);

            GUI.Label(
                new Rect(buttonBoxX + PATH_LABEL_WIDTH, buttonBoxY + offsetY, width, height),
                soundButton.Clip?.OriginalPath ?? "No file specified."
            );

            // Change button
            float changeButtonX = buttonBoxMaxX - CHANGE_BUTTON_WIDTH;
            if (GUI.Button(new Rect(changeButtonX, buttonBoxY + offsetY, CHANGE_BUTTON_WIDTH, 20), "Change"))
            {
                _changingPathForButton = soundButton;
                _tmpSelectedPath = soundButton.Clip?.OriginalPath ?? "";
            }

            return Math.Max(height, 20);
        }

        private float DrawPathBindingUI(SoundButton soundButton, float buttonBoxX, float buttonBoxY, float offsetY,
            float buttonBoxMaxX)
        {
            // Show path input field
            float width = buttonBoxMaxX - buttonBoxX - PATH_LABEL_WIDTH - CHANGE_BUTTON_WIDTH - RESET_BUTTON_WIDTH;
            _tmpSelectedPath = GUI.TextField(
                new Rect(buttonBoxX + PATH_LABEL_WIDTH, buttonBoxY + offsetY, width, 20),
                _tmpSelectedPath
            );

            // Apply button
            float buttonStart = buttonBoxMaxX - CHANGE_BUTTON_WIDTH - APPLY_BUTTON_WIDTH;
            if (GUI.Button(new Rect(buttonStart, buttonBoxY + offsetY, APPLY_BUTTON_WIDTH, 20), "Apply"))
            {
                soundButton.Clip = new MediaClip(_tmpSelectedPath);
                this.StartCoroutine(soundButton.LoadClip());
                _tmpSelectedPath = null;
                _changingPathForButton = null;
            }

            // Reset button
            if (GUI.Button(new Rect(buttonStart + APPLY_BUTTON_WIDTH, buttonBoxY + offsetY, RESET_BUTTON_WIDTH, 20),
                    "Reset"))
            {
                _changingPathForButton = null;
                _tmpSelectedPath = null;
            }

            return 20;
        }

        private float DrawNormalHotkeyUI(SoundButton soundButton, float buttonBoxX, float buttonBoxY, float offsetY,
            float buttonBoxMaxX)
        {
            // Display current hotkey
            float width = buttonBoxMaxX - buttonBoxX - HOTKEY_LABEL_WIDTH - CHANGE_BUTTON_WIDTH;
			string keys = soundButton.Hotkey.ConcatKeys().Length > 0 ? soundButton.Hotkey.ConcatKeys() : "No hotkey set.";
			GUIStyle hotkeyStyle = new GUIStyle(GUI.skin.label);
			hotkeyStyle.normal.textColor = soundButton.Hotkey.ConcatKeys().Length > 0 ? Color.white : Color.gray;
            float height = GUI.skin.label.CalcHeight(new GUIContent(keys), width);

            GUI.Label(
                new Rect(buttonBoxX + HOTKEY_LABEL_WIDTH, buttonBoxY + offsetY, width, height),
				keys,
				hotkeyStyle
            );

            // Change button
            float changeButtonX = buttonBoxMaxX - CHANGE_BUTTON_WIDTH;
            if (GUI.Button(new Rect(changeButtonX, buttonBoxY + offsetY, CHANGE_BUTTON_WIDTH, 20), "Change"))
            {
                StartHotkeyBinding(soundButton);
            }

            return Math.Max(height, 20);
        }

        private float DrawHotkeyBindingUI(SoundButton soundButton, float buttonBoxX, float buttonBoxY, float offsetY,
            float buttonBoxMaxX)
        {
            // Show currently pressed keys
            string keys = string.Join(" + ", _currentlyBindingKeys);
            float width = buttonBoxMaxX - buttonBoxX - HOTKEY_LABEL_WIDTH - CHANGE_BUTTON_WIDTH - RESET_BUTTON_WIDTH;
            float height = GUI.skin.label.CalcHeight(new GUIContent(keys), width);

            GUI.Label(
                new Rect(buttonBoxX + HOTKEY_LABEL_WIDTH, buttonBoxY + offsetY, width, height),
                keys
            );

            // Apply button
            float buttonStart = buttonBoxMaxX - CHANGE_BUTTON_WIDTH - APPLY_BUTTON_WIDTH;
            if (GUI.Button(new Rect(buttonStart, buttonBoxY + offsetY, APPLY_BUTTON_WIDTH, 20), "Apply"))
            {
                ApplyHotkeyBinding(soundButton);
            }

            // Reset button
            if (GUI.Button(new Rect(buttonStart + APPLY_BUTTON_WIDTH, buttonBoxY + offsetY, RESET_BUTTON_WIDTH, 20),
                    "Reset"))
            {
                CancelHotkeyBinding();
            }

            return Math.Max(height, 20);
        }

        private void StartHotkeyBinding(SoundButton soundButton)
        {
            _isListeningForInput = true;
            _bindingHotkeyForButton = soundButton;
            _currentlyBindingKeys.Clear();
        }

        private void ApplyHotkeyBinding(SoundButton soundButton)
        {
            soundButton.Hotkey.Keys = new List<KeyCode>(_currentlyBindingKeys);
            CancelHotkeyBinding();
        }

        private void CancelHotkeyBinding()
        {
            _isListeningForInput = false;
            _bindingHotkeyForButton = null;
            _currentlyBindingKeys.Clear();
        }
    }
}
