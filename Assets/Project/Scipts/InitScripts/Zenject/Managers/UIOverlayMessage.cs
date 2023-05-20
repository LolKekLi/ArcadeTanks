using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class UIOverlayMessage : ZenjectManager<UIOverlayMessage>
    {
        private const float HideDelay = 2f;
        private const int ScreenOffset = 80;

        [SerializeField]
        private TextMeshProUGUI _messageLabel = null;
        [SerializeField]
        private RectTransform _labelRect = null;
        [SerializeField]
        private LayoutElement _element = null;
        [SerializeField]
        private RectTransform _backgroundRect = null;

        private RectTransform _transform = null;
        
        private Sequence _appearSequence = null;
        private Sequence _hideSequence = null;

        protected override void Awake()
        {
            base.Awake();

            _transform = (RectTransform)transform;
        }

        protected override void Init()
        {
            base.Init();
            
            gameObject.SetActive(false);

            _appearSequence = DOTween.Sequence();
            _appearSequence.Append(_backgroundRect.DOScale(Vector3.one, 0.4f).From(Vector3.zero).SetEase(Ease.OutQuad).SetAutoKill(false));
            _appearSequence.SetAutoKill(false);
            _appearSequence.Restart();

            _hideSequence = DOTween.Sequence();
            _hideSequence.Append(_backgroundRect.DOScale(Vector3.zero, 0.4f).From(Vector3.one).SetEase(Ease.OutQuad).SetAutoKill(false)
                .OnComplete(
                    () =>
                    {
                        gameObject.SetActive(false);
                    }));
            _hideSequence.SetAutoKill(false);
            _hideSequence.Restart();
        }

        public void Show(string text)
        {
            if (gameObject.activeSelf)
            {
                return;
            }
            
            _messageLabel.text = text;

            gameObject.SetActive(true);
            
            _appearSequence.Restart();
            _appearSequence.PlayForward();

            RebuildAsync();

            HideAsync();
        }

        private async void HideAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(HideDelay));
            
            _hideSequence.Restart();
            _hideSequence.PlayForward();
        }
        
        private async void RebuildAsync()
        {
            await UniTask.WaitForEndOfFrame();

            float screenWidth = _transform.rect.width - ScreenOffset;

            if (_labelRect.rect.width > screenWidth)
            {
                _element.enabled = true;
                _element.preferredWidth = screenWidth;
            }
        }
    }
}