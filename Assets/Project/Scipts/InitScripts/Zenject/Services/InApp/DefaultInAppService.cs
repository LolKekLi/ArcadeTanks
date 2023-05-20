using System;

namespace Project.Service
{
    public class DefaultInAppService : IInAppService
    {
        public event Action<bool> Subscribed;

        public void Init() { }

        public void ShowPopup()
        {
            ((IInAppService)this).OnSubscribe(true);
        }

        void IInAppService.OnSubscribe(bool isSuccess)
        {
            Subscribed?.Invoke(isSuccess);
        }
    }
}