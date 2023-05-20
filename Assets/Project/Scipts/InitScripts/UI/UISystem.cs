using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Project.UI
{
    public class UISystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject _windowsContainer = null;

        [SerializeField]
        private Camera _camera = null;

        private Window[] _windows = null;

        private Window _current = null;
        private Stack<Window> _stack = new Stack<Window>();

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        private bool IsManyWindowOpened
        {
            get => _stack.Count <= 1;
        }

        public Window CurrentWindow
        {
            get => _current;
        }

        public Camera Camera
        {
            get => _camera;
        }

        public Dictionary<string, object> Data
        {
            get => _data;
        }

        private void Awake()
        {
            _windows = _windowsContainer.GetComponentsInChildren<Window>(true);
            _windows.Do(wind => wind.Preload());

            DontDestroyOnLoad(gameObject);
        }

        public async void ShowWindow<T>(Dictionary<string, object> data = null)
            where T : Window
        {
            var window = GetWindow<T>();

            if (_current == window)
            {
                return;
            }

            SetData(data);

            if (!window.IsPopup)
            {
                foreach (var wnd in _stack)
                {
                    await wnd.Hide(IsManyWindowOpened);
                }

                _stack.Clear();
            }
            else
            {
                window.transform.SetAsLastSibling();
            }

            _stack.Push(window);

            window.Show();

            _current = window;
        }

        public void ReturnToPreviousWindow()
        {
            if (_stack.Count > 0)
            {
                var window = _stack.Pop();
                window.Hide(true);
            }
        }

        private Window GetWindow<T>()
            where T : Window
        {
            var window = _windows.FirstOrDefault(w => w is T);

            if (!window)
            {
                DebugSafe.LogException(new Exception($"{typeof(T)} not found!"));
            }

            return window;
        }

        private void SetData(Dictionary<string, object> data)
        {
            if (data != null)
            {
                foreach (var record in data)
                {
                    if (_data.ContainsKey(record.Key))
                    {
                        _data[record.Key] = record.Value;
                    }
                    else
                    {
                        _data.Add(record.Key, record.Value);
                    }
                }
            }
        }
        
        [Conditional("FORCE_DEBUG")]
        public void ToggleUI(bool isActive)
        {
            foreach (var wnd in _windows)
            {
                if (!wnd.TryGetComponent(out CanvasGroup canvasGroup))
                {
                    canvasGroup = wnd.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.alpha = isActive ? 1 : 0;
            }
        }
    }
}