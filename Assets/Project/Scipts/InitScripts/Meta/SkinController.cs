using System;
using System.Linq;
using Project.Service;
using Zenject;

namespace Project.Meta
{
    public class SkinController
    {
        public event Action Selected = delegate { };
        public event Action Unlocked = delegate { };

        private IAdvertisingService _advertisingService = null;
        private IInAppService _inAppService = null;

        public SkinSettings SkinSettings
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(SkinSettings skinSettings, IAdvertisingService advertisingService, IInAppService inAppService)
        {
            SkinSettings = skinSettings;
            _advertisingService = advertisingService;
            _inAppService = inAppService;
        }

        public void OnClick(SkinPreset skinPreset, Action callback)
        {
            if (IsUnlocked(skinPreset))
            {
                Select(skinPreset);
            }
            else if (skinPreset.UnlockType == UnlockType.Ads)
            {
                _advertisingService.ShowRewarded(AdvertisingPlacement.ShopItem, isSuccess =>
                {
                    if (isSuccess)
                    {
                        LocalConfig.SetSkinClaimProgress(skinPreset.SkinType, skinPreset.SkinPartType);

                        var claimProgress =
                            LocalConfig.GetSkinClaimProgress(skinPreset.SkinType, skinPreset.SkinPartType);
                        
                        if (claimProgress >= skinPreset.SkinUnlockCount)
                        {
                            Unlock(skinPreset);
                        }
                    }
                });
            }
            else
            {
                _inAppService.ShowPopup();
            }

            callback?.Invoke();
        }

        public void UnlockRandom(SkinPartType skinPartType)
        {
            var skinPresets = SkinSettings.GetSkinPresets(skinPartType)
                .Where(IsAvailableToUnlock).ToList();

            var skinPreset = skinPresets.RandomElement();
            
            Unlock(skinPreset);
        }

        public bool IsAvailableToUnlock(SkinPreset skinPreset)
        {
            return !IsUnlocked(skinPreset) && (skinPreset.RarityType != RarityType.Vip || !InAppHelper.IsInAppEnabled) &&
                   skinPreset.UnlockType != UnlockType.Ads;
        }

        public bool IsUnlocked(SkinPreset skinPreset)
        {
            return LocalConfig.IsSkinUnlocked(skinPreset.SkinType, skinPreset.SkinPartType);
        }

        public bool IsSelected(SkinPreset skinPreset)
        {
            return LocalConfig.IsSkinSelected(skinPreset.SkinType, skinPreset.SkinPartType);
        }

        private void Select(SkinPreset skinPreset)
        {
            LocalConfig.SelectSkin(skinPreset.SkinType, skinPreset.SkinPartType);

            Selected?.Invoke();
        }

        private void Unlock(SkinPreset skinPreset)
        {
            LocalConfig.UnlockSkin(skinPreset.SkinType, skinPreset.SkinPartType, true);

            Unlocked?.Invoke();
            
            Select(skinPreset);
        }

        private void Remove(SkinPreset skinPreset)
        {
            LocalConfig.UnlockSkin(skinPreset.SkinType, skinPreset.SkinPartType, false);
        }
    }
}