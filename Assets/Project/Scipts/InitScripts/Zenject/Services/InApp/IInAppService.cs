using System;

namespace Project.Service
{
    public interface IInAppService
    {
        public event Action<bool> Subscribed;

        void Init();
        void ShowPopup();
        void OnSubscribe(bool isSuccess);
    }
}