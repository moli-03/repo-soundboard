using System;
using UnityEngine;

namespace REPOSoundBoard.UI.Utils
{
    public class IMGUIWindow
    {
        private Rect _windowRect;
        private string _title;
        private Action<int> _drawContent;
        private bool _isDraggable = true;
        private int _id;
        private static int _nextId = 0;

        public Rect WindowRect => _windowRect;

        public IMGUIWindow(string title, Rect initialRect, Action<int> drawContent)
        {
            this._title = title;
            this._windowRect = initialRect;
            this._drawContent = drawContent;
            this._id = _nextId++;
        }

        public void SetDraggable(bool isDraggable)
        {
            this._isDraggable = isDraggable;
        }

        public void Draw()
        {
            if (_isDraggable)
            {
                _windowRect = GUILayout.Window(_id, _windowRect, WindowFunction, _title);
            }
            else
            {
                IMGUIUtils.AreaGroup(_windowRect, () => {
                    GUILayout.Label(_title, GUI.skin.box, GUILayout.ExpandWidth(true));
                    _drawContent?.Invoke(_id);
                });
            }
        }

        private void WindowFunction(int windowId)
        {
            _drawContent?.Invoke(windowId);
        
            if (_isDraggable)
            {
                GUI.DragWindow();
            }
        }
    }
}