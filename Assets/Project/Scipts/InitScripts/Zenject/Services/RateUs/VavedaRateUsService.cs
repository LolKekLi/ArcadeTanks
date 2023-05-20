#if Vaveda
using Uni.Wrapper;
#endif

namespace Project.Service
{
    public class VavedaRateUsService : IRateUsService
    {
        public void ShowRateUs()
        {
#if Vaveda
            UniWrapper.RateUsManager?.ShowRateUs();            
#endif
        }
    }
}