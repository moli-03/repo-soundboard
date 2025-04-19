using System;
using System.Collections.Generic;
using System.Linq;
using REPOSoundBoard.Core;
using REPOSoundBoard.UI.Utils;
using UnityEngine;

namespace REPOSoundBoard.UI.Components
{
    public class SoundButtonsUI
    {

        private string _search = string.Empty;
        private Vector2 _scrollPosition = Vector2.zero;

        private List<SoundButtonUI> _soundButtonUis;
        private SoundButtonUI _buttonToDelete = null;
        
        public SoundButtonsUI()
        {
            _soundButtonUis = SoundBoard.Instance.SoundButtons.Select(button =>
            {
                var buttonUI = new SoundButtonUI(button);
                buttonUI.OnDeleteClicked += this.HandleDeleteButtonPress;
                return buttonUI;
            }).ToList();
        }

        private void HandleDeleteButtonPress(SoundButtonUI buttonUI)
        {
            _buttonToDelete = buttonUI;
        }

        private void HandleCurrentDeletion()
        {
            if (_buttonToDelete == null)
            {
                return;
            }
            
            this._soundButtonUis.Remove(_buttonToDelete);
            SoundBoard.Instance.RemoveSoundButton(_buttonToDelete.SoundButton);
        }
        
        public void Draw()
        {
            HandleCurrentDeletion();
            
            _search = IMGUIUtils.LabeledTextField("Search:", _search).ToLower();

            _scrollPosition = IMGUIUtils.ScrollGroup(_scrollPosition, () =>
            {
                foreach (var soundButtonUI in _soundButtonUis.Where(buttonUI => buttonUI.SoundButton.Name.ToLower().Contains(_search)))
                {
                    soundButtonUI.Draw();
                }
            });

            if (GUILayout.Button("Add new sound button", GUILayout.ExpandWidth(true)))
            {
                var button = new SoundButton();
                var buttonUI = new SoundButtonUI(button, true);
                buttonUI.OnDeleteClicked += this.HandleDeleteButtonPress;
                SoundBoard.Instance.AddSoundButton(button);
                this._soundButtonUis.Insert(0, buttonUI);
            }
        }
    }
}