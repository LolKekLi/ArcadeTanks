using System;

namespace Project.Service
{
    public interface IAdvertisingService
    {
        void Init();
        void ShowRewarded(string place, Action<bool> callback);
        bool IsRewardedAvailable(string place);
        void ShowInterstitial(string place);
        bool IsInterstitialAvailable(string place);
        void ShowBanner();
        void HideBanner();
    }
}