using System;
using Zenject;

namespace Project.Service
{
    public static class AdvertisingServiceController
    {
        public static IAdvertisingService GetService(InjectContext context)
        {
            var targetStore = CompanyHelper.GetTargetCompany();
            IAdvertisingService advertisingService = null;

            switch (targetStore)
            {
                case CompanyType.None:
                    advertisingService = context.Container.Instantiate<DefaultAdvertisingService>();
                    break;
                
                case CompanyType.Vaveda:
                    advertisingService = context.Container.Instantiate<VavedaAdvertisingService>();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(IAdvertisingService)} for {nameof(CompanyType)}: {targetStore}"));
                    break;
            }
            
            advertisingService?.Init();

            return advertisingService;
        }
    }
}