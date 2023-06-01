using System;
using Project.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public abstract class UISelector<T> : MonoBehaviour where T : Enum
    {
        
        [SerializeField]
        private Button _leftButton;

        [SerializeField]
        private Button _rightButton;

        [SerializeField]
        private Image _back;

        [SerializeField]
        private Image _icon;
        
        protected ReactiveProperty<int> _index = new ReactiveProperty<int>(0);
        protected ReactiveProperty<T> _currentType = new ReactiveProperty<T>();
        protected UIIconSettings.UIIconPreset<T>[] _selectorPresets;
        
        [Inject]
        protected IUser _user;
        [Inject]
        protected UIIconSettings _uiIconSettings;

        public IReadOnlyReactiveProperty<T> CurrentType
        {
            get =>
                _currentType;
        }

        private void Awake()
        {
            Prepare();
        }

        protected virtual void Start()
        {
            _leftButton.onClick.AddListener(() =>
            {
                var indexValue = _index.Value;
                indexValue--;

                if (indexValue < 0)
                {
                    _index.Value = _selectorPresets.Length - 1;
                }
                else
                {
                    _index.Value = indexValue;
                }
            }, SoundType.Click);

            _rightButton.onClick.AddListener(() =>
            {
                var indexValue = _index.Value;
                indexValue++;

                if (indexValue >= _selectorPresets.Length)
                {
                    _index.Value = 0;
                }
                else
                {
                    _index.Value = indexValue;
                }
            }, SoundType.Click);
            
            _index.Subscribe(index =>
            {
                ChangeIcon();
            });
        }

        private void ChangeIcon()
        {
            _icon.sprite = _selectorPresets[_index.Value].Icon;
            _currentType.Value = _selectorPresets[_index.Value].Type;
        }

        protected abstract void Prepare();
    }
}