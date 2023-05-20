using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    public class ToggleButton : Button
    {
        [SerializeField]
        private Image _image = null;

        [SerializeField]
        private TextMeshProUGUI _text = null;

        [SerializeField]
        private Sprite _activeSprite = null;

        [SerializeField]
        private Sprite _disabledSprite = null;

        private Func<bool> _func = null;
        private Func<string> _textFunc = null;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            OnButtonClick();
        }

        public void Setup(bool isActive, Func<bool> func, Func<string> textFunc = null)
        {
            _func = func;
            _textFunc = textFunc;
            
            SetupImage(isActive);
            UpdateText();
        }

        private void SetupImage(bool isActive)
        {
            _image.sprite = isActive ? _activeSprite : _disabledSprite;
        }

        private void UpdateText()
        {
            if (_textFunc != null)
            {
                _text.text = _textFunc.Invoke();
            }
        }

        private void OnButtonClick()
        {
            SetupImage(_func.Invoke());

            UpdateText();
        }
    }
}