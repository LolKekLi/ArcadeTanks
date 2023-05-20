using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.UIDebug
{
    public class HotKeyHelper
    {
        private Dictionary<KeyCode, Action> _dictionary = null;

        public HotKeyHelper(Dictionary<KeyCode, Action> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Tick()
        {
            foreach (var pair in _dictionary)
            {
                if (Input.GetKeyDown(pair.Key))
                {
                    _dictionary[pair.Key]?.Invoke();
                }
            }
        }
    }
}