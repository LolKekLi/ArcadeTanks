using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public abstract class Window : MonoBehaviour
    {
        public static event Action<Window> Shown = delegate { };
        public static event Action<Window> Hidden = delegate { };

        [SerializeField]
        private Button _backButton = null;

        [SerializeField]
        private SelfTweenController _onShowAnimation = null;

        [SerializeField]
        private SelfTweenController _onHideAnimation = null;

        private User _user = null;
        private UISystem _uiSystem = null;
        
        private UniRxSubscribersContainer _subscribersContainer = new UniRxSubscribersContainer();
      
        public abstract bool IsPopup
        {
            get;
        }

        public float HideTime
        {
            get
            {
                if (_onHideAnimation == null)
                {
                    return 0f;
                }

                return _onHideAnimation.LongestAnimationTime;
            }
        }

        [Inject]
        private void Construct(User user, UISystem uiSystem)
        {
            _user = user;
            _uiSystem = uiSystem;
        }

        protected virtual void OnEnable()
        {
            _subscribersContainer.Subscribe(_user.Coins, User_CurrencyChanged);
        }

        protected virtual void OnDisable()
        {
            _subscribersContainer.FreeSubscribers();
        }

        protected virtual void Start()
        {
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }
        }

        public void Show()
        {
            OnShow();

            Shown(this);

            AfterShown();

            Refresh();
        }

        public async UniTask Hide(bool isAnimationNeeded)
        {
            await OnHide(isAnimationNeeded);

            Hidden(this);
        }

        public virtual void Preload()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnShow()
        {
            gameObject.SetActive(true);

            if (_onShowAnimation != null)
            {
                _onShowAnimation.Play();
            }
        }

        protected virtual void AfterShown()
        {
        }

        protected virtual void Refresh()
        {
        }

        protected virtual async UniTask OnHide(bool isAnimationNeeded)
        {
            if (isAnimationNeeded)
            {
                if (_onHideAnimation != null)
                {
                    _onHideAnimation.Play();

                    await UniTask.Delay(TimeSpan.FromSeconds(_onHideAnimation.LongestAnimationTime));
                }
            }

            gameObject.SetActive(false);
        }

        protected T GetDataValue<T>(string itemKey, T defaultValue = default,
            Dictionary<string, object> forcedData = null)
        {
            Dictionary<string, object> data = _uiSystem.Data;

            if (data == null || data.Count == 0)
            {
                return defaultValue;
            }

            if (!data.TryGetValue(itemKey, out object itemObject))
            {
                return defaultValue;
            }

            if (itemObject is T)
            {
                return (T)itemObject;
            }

            return defaultValue;
        }

        protected void SetDataValue<T>(string itemKey, T value)
        {
            if (!_uiSystem.Data.ContainsKey(itemKey))
            {
                _uiSystem.Data.Add(itemKey, value);
            }
            else
            {
                _uiSystem.Data[itemKey] = value;
            }
        }

        protected virtual void OnBackButtonClicked()
        {
            _uiSystem.ReturnToPreviousWindow();
        }

        private void User_CurrencyChanged(int value)
        {
            Refresh();
        }
    }
}