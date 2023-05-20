using System;
using Zenject;

namespace Project.Service
{
    public static class RateUsServiceController
    {
        public static IRateUsService GetService(InjectContext context)
        {
            var targetStore = CompanyHelper.GetTargetCompany();
            IRateUsService advertisingService = null;

            switch (targetStore)
            {
                case CompanyType.None:
                    advertisingService = context.Container.Instantiate<DefaultRateUsService>();
                    break;
                
                case CompanyType.Vaveda:
                    advertisingService = context.Container.Instantiate<VavedaRateUsService>();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(IRateUsService)} for {nameof(CompanyType)}: {targetStore}"));
                    break;
            }

            return advertisingService;
        }

    }
}