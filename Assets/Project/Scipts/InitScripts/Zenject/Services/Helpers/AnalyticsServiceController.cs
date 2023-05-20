using System;
using Zenject;

namespace Project.Service
{
    public static class AnalyticsServiceController 
    {
        public static IAnalyticsService GetService(InjectContext context)
        {
            var targetStore = CompanyHelper.GetTargetCompany();
            IAnalyticsService advertisingService = null;

            switch (targetStore)
            {
                case CompanyType.None:
                    advertisingService = context.Container.Instantiate<DefaultAnalyticsService>();
                    break;
                
                case CompanyType.Vaveda:
                    advertisingService = context.Container.Instantiate<VavedaAnalyticsService>();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(IAnalyticsService)} for {nameof(CompanyType)}: {targetStore}"));
                    break;
            }

            return advertisingService;
        }
    }
}