using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public abstract class RealoaderBase
    {
        protected Image _reloadImage;
        protected CancellationTokenSource _fillToken;

        public RealoaderBase(Image reloadImage)
        {
            _reloadImage = reloadImage;
        }

        public abstract void OnFire();

        protected async void FillReloadImage(float fillTime, float targetValue, CancellationToken cancellationToken, Action callback = null)
        {
            try
            {
                var startValue = _reloadImage.fillAmount;
                
                await UniTaskExtensions.Lerp(time =>
                    {
                        _reloadImage.fillAmount = Mathf.Lerp(startValue, targetValue, time);
                    },
                    fillTime, token: cancellationToken);
                
                callback?.Invoke();
            }
            catch (OperationCanceledException e)
            {
                callback?.Invoke();
            }
        }

        public virtual void CanselToken()
        {
            UniTaskUtil.CancelToken(ref _fillToken);
        }

        public virtual void OnStopFire(bool isOverhead)
        {
        }
    }
}