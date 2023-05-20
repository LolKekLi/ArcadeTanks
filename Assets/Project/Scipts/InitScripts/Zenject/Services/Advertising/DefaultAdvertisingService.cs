using System;

namespace Project.Service
{
    public class DefaultAdvertisingService : IAdvertisingService
    {
        public void Init() { }

        public void ShowRewarded(string place, Action<bool> callback)
        {
            if (IsRewardedAvailable(place))
            {
                callback?.Invoke(true);
                
                DebugSafe.Log($"ShowRewarded. Place - {place}.");
            }
        }

        public bool IsRewardedAvailable(string place)
        {
            return true;
        }

        public void ShowInterstitial(string place)
        {
            if (IsInterstitialAvailable(place))
            {
                DebugSafe.Log($"ShowInterstitial. Place - {place}.");
            }
        }

        public bool IsInterstitialAvailable(string place)
        {
            return true;
        }

        public void ShowBanner()
        {
            DebugSafe.Log($"Show Banner.");
        }

        public void HideBanner()
        {
            DebugSafe.Log($"Hide Banner.");
        }
    }
}