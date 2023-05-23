using Project.Meta;
using Zenject;

namespace Project
{
    public class HubViewModel : TankViewModel
    {
        [Inject]
        private IUser user;
       
        private UniRxSubscribersContainer _subscribersContainer = new UniRxSubscribersContainer();
        
        private void OnEnable()
        {
            _subscribersContainer.Subscribe(user.BodyType, type =>
            {
                Setup(user.TurretType.Value, user.BodyType.Value);
            });
            _subscribersContainer.Subscribe(user.TurretType, type =>
            {
                Setup(user.TurretType.Value, user.BodyType.Value);
            });
        }

        private void OnDisable()
        {
            _subscribersContainer.FreeSubscribers();
        }
        
        protected override void Awake()
        {
            base.Awake();
            Setup(user.TurretType.Value, user.BodyType.Value);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
        }
#endif
    }
}