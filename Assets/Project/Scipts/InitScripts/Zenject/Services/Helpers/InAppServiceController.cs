using System;
using Zenject;

namespace Project.Service
{
    public static class InAppServiceController
    {
        public static IInAppService GetService(InjectContext context)
        {
            var targetStore = CompanyHelper.GetTargetCompany();
            IInAppService advertisingService = null;

            switch (targetStore)
            {
                case CompanyType.None:
                    advertisingService = context.Container.Instantiate<DefaultInAppService>();
                    break;
                
                case CompanyType.Vaveda:
                    advertisingService = context.Container.Instantiate<VavedaInAppService>();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(IInAppService)} for {nameof(CompanyType)}: {targetStore}"));
                    break;
            }
            
            advertisingService?.Init();

            return advertisingService;
        }
    }
}