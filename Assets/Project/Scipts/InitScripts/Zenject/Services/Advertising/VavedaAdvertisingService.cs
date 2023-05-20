using System;
using Cysharp.Threading.Tasks;

#if Vaveda
using Uni.Wrapper;
#endif

namespace Project.Service
{
    public class VavedaAdvertisingService : IAdvertisingService
    {
        private Action<bool> _callback = null;
        
        public void Init()
        {
#if Vaveda
            InitAsync();
#endif
        }
        
#if Vaveda
        private async void InitAsync()
        {
            while (!UniWrapper.IsInitialized)
            {
                await UniTask.Yield();
            }
            
            UniWrapper.AdsManager.OnRewardedStateClose += AdsManager_OnRewardedStateClose;
        } 
#endif

        public void ShowRewarded(string place, Action<bool> callback)
        {
            if (IsRewardedAvailable(place))
            {
                _callback = callback;
                
#if Vaveda
                UniWrapper.AdsManager?.ShowRewardedVideo(place);
#endif
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public bool IsRewardedAvailable(string place)
        {
#if Vaveda
            return UniWrapper.AdsManager?.IsRewardedAvailable(place) ?? false;
#else
            return true;
#endif
        }

        public void ShowInterstitial(string place)
        {
            if (IsInterstitialAvailable(place))
            {
#if Vaveda
                UniWrapper.AdsManager?.ShowInterstitial(place);
#endif
            }
        }

        public bool IsInterstitialAvailable(string place)
        {
#if Vaveda
            return UniWrapper.AdsManager?.IsInterstitialAvailable(place) ?? false;
#else
            return true;
#endif
        }

        public void ShowBanner()
        {
#if Vaveda
            UniWrapper.AdsManager?.ShowBanner();
#endif
        }

        public void HideBanner()
        {
#if Vaveda
            UniWrapper.AdsManager?.HideBanner();
#endif
        }
        
        private void AdsManager_OnRewardedStateClose(bool isCompleted)
        {
            _callback?.Invoke(isCompleted);
        }
    }
}