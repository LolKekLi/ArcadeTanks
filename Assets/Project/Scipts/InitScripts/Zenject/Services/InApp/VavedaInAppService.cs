using System;
using Zenject;

#if Vaveda
using JambaEngine.Purchasing;
using Uni.Wrapper;
using UnityEngine.Purchasing;
#endif

namespace Project.Service
{
#if Vaveda
    public class CustomJambaListener : JambaSubscriptionListener
    {
        public event Action Shown = null;

        public override void ShowSubscriptionDialog()
        {
            base.ShowSubscriptionDialog();

            Shown?.Invoke();
        }
    }
#endif
    
    public class VavedaInAppService : IInAppService
    {
        private IAdvertisingService _advertisingService;
#if Vaveda
        private CustomJambaListener _listener;
#endif
        
        public event Action<bool> Subscribed;

        [Inject]
        private void Construct(IAdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
        }
        
        public void Init()
        {
#if Vaveda
            JambaStartAppPurchasing.OnSubscriptionUpdated += JambaStartAppPurchasing_OnSubscriptionUpdated;
            JambaStartAppPurchasing.OnHideSubscriptionDialog += JambaStartAppPurchasing_OnHideSubscriptionDialog;
            
            _listener = new CustomJambaListener();
            
            JambaStartAppPurchasing.Initialize(_listener);
            
            _listener.Shown += CustomJambaListener_Shown;
#endif
        }

        public void ShowPopup()
        {
#if Vaveda
            if (JambaStartAppPurchasing.IsCanShow)
            {
                JambaStartAppPurchasing.ShowSubscriptionDialog();
            }
#endif
        }

        void IInAppService.OnSubscribe(bool isSuccess)
        {
            Subscribed?.Invoke(isSuccess);
        }

#if Vaveda
        private void JambaStartAppPurchasing_OnSubscriptionUpdated(bool isSuccess, PurchaseEventArgs arg2)
        {
            if (isSuccess)
            {
                UniWrapper.PurchasingManager.EnablePremium();
            }
            else
            {
                UniWrapper.PurchasingManager.DisablePremium();
            }
        }

        private void JambaStartAppPurchasing_OnHideSubscriptionDialog()
        {
            _advertisingService.ShowBanner();
        }
        
        private void CustomJambaListener_Shown()
        {
            _advertisingService.HideBanner();
        }
#endif
    }
}