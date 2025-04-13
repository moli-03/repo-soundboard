using System;
using UnityEngine;

namespace REPOSoundBoard.UI.Utils
{
    public class IMGUITabs
    {
        private string[] tabs;
        private int selectedTab;
        private Action<int>[] tabContents;
        private GUIStyle tabStyle;
        private GUIStyle activeTabStyle;

        public int SelectedTab => selectedTab;

        public IMGUITabs(string[] tabs, Action<int>[] tabContents)
        {
            this.tabs = tabs;
            this.tabContents = tabContents;
        
            // Default tab style
            tabStyle = new GUIStyle(GUI.skin.button);
        
            // Active tab style
            activeTabStyle = new GUIStyle(GUI.skin.button);
            activeTabStyle.normal.background = activeTabStyle.active.background;
        }

        public void SetStyles(GUIStyle tabStyle, GUIStyle activeTabStyle)
        {
            this.tabStyle = tabStyle;
            this.activeTabStyle = activeTabStyle;
        }

        public void Draw()
        {
            // Draw tab headers
            IMGUIUtils.HorizontalGroup(() => {
                for (int i = 0; i < tabs.Length; i++)
                {
                    bool isSelected = (i == selectedTab);
                    if (GUILayout.Button(tabs[i], isSelected ? activeTabStyle : tabStyle))
                    {
                        selectedTab = i;
                    }
                }
            });

            // Draw tab content
            IMGUIUtils.BoxGroup(() => {
                if (selectedTab >= 0 && selectedTab < tabContents.Length)
                {
                    tabContents[selectedTab]?.Invoke(selectedTab);
                }
            });
        }
    }
}