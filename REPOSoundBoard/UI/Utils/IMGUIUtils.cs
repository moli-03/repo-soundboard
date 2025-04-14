using System;
using REPOSoundBoard.Core.Hotkeys;
using UnityEngine;

namespace REPOSoundBoard.UI.Utils
{
    public class IMGUIUtils
    {
        #region Styles

        public static readonly GUIStyle DisabledTextStyle;
        public static readonly GUIStyle HeadingStyle;

        static IMGUIUtils()
        {
			DisabledTextStyle = GUI.skin.label;
            DisabledTextStyle.normal.textColor = new Color(120, 120, 120);
            
			HeadingStyle = new GUIStyle(GUI.skin.label);
            HeadingStyle.normal.textColor = Color.white;
            HeadingStyle.fontSize = 15;
        }
        
        #endregion
        
        #region Layout Helpers
        
        /// <summary>
        /// Creates a horizontal layout group
        /// </summary>
        public static void HorizontalGroup(Action guiAction)
        {
            GUILayout.BeginHorizontal();
            guiAction?.Invoke();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a vertical layout group
        /// </summary>
        public static void VerticalGroup(Action guiAction)
        {
            GUILayout.BeginVertical();
            guiAction?.Invoke();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Creates a scrollable area
        /// </summary>
        public static Vector2 ScrollGroup(Vector2 scrollPosition, Action guiAction, GUIStyle style = null)
        {
            Vector2 newScrollPosition = GUILayout.BeginScrollView(scrollPosition, style ?? GUI.skin.scrollView);
            guiAction?.Invoke();
            GUILayout.EndScrollView();
            return newScrollPosition;
        }
        
        /// <summary>
        /// Creates a box container
        /// </summary>
        public static void BoxGroup(Action guiAction, GUIStyle style = null)
        {
            GUILayout.BeginVertical(style ?? GUI.skin.box);
            guiAction?.Invoke();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Creates an area with specified dimensions
        /// </summary>
        public static void AreaGroup(Rect rect, Action guiAction)
        {
            GUILayout.BeginArea(rect);
            guiAction?.Invoke();
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Style helpers
        
        /// <summary>
        /// Temporarily changes the GUI color for a block of code
        /// </summary>
        public static void WithColor(Color color, Action guiAction)
        {
            Color originalColor = GUI.color;
            GUI.color = color;
            guiAction?.Invoke();
            GUI.color = originalColor;
        }

        /// <summary>
        /// Temporarily changes the GUI content color for a block of code
        /// </summary>
        public static void WithContentColor(Color color, Action guiAction)
        {
            Color originalColor = GUI.contentColor;
            GUI.contentColor = color;
            guiAction?.Invoke();
            GUI.contentColor = originalColor;
        }

        /// <summary>
        /// Temporarily changes the GUI background color for a block of code
        /// </summary>
        public static void WithBackgroundColor(Color color, Action guiAction)
        {
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            guiAction?.Invoke();
            GUI.backgroundColor = originalColor;
        }

        /// <summary>
        /// Temporarily changes the GUI enabled state for a block of code
        /// </summary>
        public static void WithEnabled(bool enabled, Action guiAction)
        {
            bool originalState = GUI.enabled;
            GUI.enabled = enabled;
            guiAction?.Invoke();
            GUI.enabled = originalState;
        }
        
        #endregion
        
        #region Control Helpers
        
		/// <summary>
		/// Creates a labeled slider
		/// </summary>
		public static float LabeledSlider(string label, float value, float leftValue, float rightValue)
		{
			HorizontalGroup(() => {
				GUILayout.Label(label, GUILayout.ExpandWidth(false), GUILayout.Height(20));
				value = GUILayout.HorizontalSlider(value, leftValue, rightValue);
				GUILayout.Label(value.ToString("F2"), GUILayout.Width(50), GUILayout.Height(20));
			});
			return value;
		}

        /// <summary>
        /// Creates a labeled field
        /// </summary>
        public static string LabeledTextField(string label, string text)
        {
            HorizontalGroup(() => {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                text = GUILayout.TextField(text);
            });
            return text;
        }

        /// <summary>
        /// Creates a labeled toggle
        /// </summary>
        public static bool LabeledToggle(string label, bool value)
        {
            HorizontalGroup(() => {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                value = GUILayout.Toggle(value, "");
            });
            return value;
        }

        /// <summary>
        /// Create a labeled hotkey input
        /// </summary>
        /// <returns></returns>
        public static bool LabeledHotkeyInput(string label, Hotkey hotkey, bool selecting)
        {
            HorizontalGroup(() =>
            {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));

                if (!selecting)
                {
                    if (hotkey.Keys.Count == 0)
                    {
                        GUILayout.Label("No hotkey selected", DisabledTextStyle);
                    }
                    else
                    {
                        GUILayout.Label(string.Join(" + ", hotkey.Keys), GUILayout.ExpandWidth(true));
                    }
                    
                    if (GUILayout.Button("Change", GUILayout.Width(60)))
                    {
                        selecting = true;
                        GUIUtility.keyboardControl = 0; // Force unfocus on input fields to make selection work
                    }
                }
                else
                {
                    var pressedKeys = HotkeySelectorUtils.GetPressedKeys();

                    if (pressedKeys.Count == 0)
                    {
                        GUILayout.Label("Start pressing keys...", DisabledTextStyle, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        GUILayout.Label(string.Join(" + ", HotkeySelectorUtils.GetPressedKeys()), GUILayout.ExpandWidth(true));
                        
                        if (GUILayout.Button("Apply", GUILayout.Width(60)))
                        {
                            hotkey.Keys = HotkeySelectorUtils.GetPressedKeys();
                            selecting = false;
                        }
                    }
                    
                    if (GUILayout.Button("Reset", GUILayout.Width(60)))
                    {
                        selecting = false;
                    }
                }
            });
            
            return selecting;
        }


        public static string LabeledPathInput(string label, string originalPath, string inputPath, ref bool changing, Action<string> pathChanged)
        {
            bool localChanging = changing;
            string localInputPath = inputPath;
            HorizontalGroup(() =>
            {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));

                if (!localChanging)
                {
                    GUILayout.Label(originalPath, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Change", GUILayout.Width(60)))
                    {
                        localChanging = true;
                    }
                }
                else
                {
                    localInputPath = GUILayout.TextField(localInputPath, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Apply", GUILayout.Width(60)))
                    {
                        pathChanged?.Invoke(localInputPath);
                        localChanging = false;
                    }
                    
                    if (GUILayout.Button("Reset", GUILayout.Width(60)))
                    {
                        localInputPath = originalPath;
                        localChanging = false;
                    }
                }
            });
            
            changing = localChanging;

            return localInputPath;
        }
        

        /// <summary>
        /// Creates a button with confirmation
        /// </summary>
        public static bool ConfirmButton(string buttonText, string confirmationText, ref bool confirmationShown)
        {
            bool result = false;
            bool localConfirmationShown = confirmationShown;
        
            if (!localConfirmationShown)
            {
                if (GUILayout.Button(buttonText))
                {
                    localConfirmationShown = true;
                }
            }
            else
            {
                HorizontalGroup(() => {
                    GUILayout.Label(confirmationText);
                    if (GUILayout.Button("Yes"))
                    {
                        result = true;
                        localConfirmationShown = false;
                    }
                    if (GUILayout.Button("No"))
                    {
                        localConfirmationShown = false;
                    }
                });
            }
        
            confirmationShown = localConfirmationShown;
            return result;
        }
        
        #endregion
    }
}