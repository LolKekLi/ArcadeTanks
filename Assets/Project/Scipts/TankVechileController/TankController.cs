using Project.Scipts.TankVechileController;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TankController : MonoBehaviour, ITank
    {
        [SerializeField]
        private TurretType _currentTurretType;
        [SerializeField]
        private BodyType _currentBodyType;
        
        [SerializeField, Space]
        private TankMovement _tankMovement;
        [SerializeField]
        private TurretMovement _turretMovement;
        [SerializeField]
        private TankViewModel _tankViewModel;
       
        private AttackControllerBase _attackController;
        
        [Inject]
        private CameraController _cameraController;
        
        [Inject]
        private AttackControllerFactory _attackControllerFactory;

        [Inject]
        private TankFireSettings _tankFireSettings;

        [Inject]
        private BulletFactory _bulletFactory;
        
        private void Awake()
        {
            Setup(_currentTurretType, _currentBodyType);
        }
        
        public TurretType Type
        {
            get => _currentTurretType;
        }

        private void Setup(TurretType turretType, BodyType bodyType)
        {
            _currentBodyType = bodyType;
            _currentTurretType = turretType;

            _tankViewModel.Setup(_currentTurretType, _currentBodyType);

            _turretMovement.Setup(_tankViewModel.TurretGameObject, _tankViewModel.FirePosition);
            _cameraController.Setup(_turretMovement, transform);

            _attackController = _attackControllerFactory.GetAttackController(turretType);
            _attackController.Setup(_tankFireSettings, _tankViewModel.FirePosition.transform, _bulletFactory);
        }
        
        private void Update()
        {
            if (Input.GetMouseButton(0) && _attackController.CanFire)
            {
                Debug.Log("Fire");
                _attackController.Fire();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("StopAttack");
                _attackController.StopFire();
            }
        }
    }
}