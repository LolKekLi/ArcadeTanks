using Project.Meta;
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
        [Inject]
        private IUser _user;
        [Inject]
        private TankBodySettings _tankBodySettings;

        private void Awake()
        {
            Setup(_user.TurretType.Value, _user.BodyType.Value);
        }

        public TurretType Type
        {
            get =>
                _currentTurretType;
        }

        private void Setup(TurretType turretType, BodyType bodyType)
        {
            _currentBodyType = bodyType;
            _currentTurretType = turretType;
            var tankMovementPreset = _tankBodySettings.GetPresetByType(_currentBodyType);
            _tankMovement.Setup(tankMovementPreset);
            _tankViewModel.Setup(_currentTurretType, _currentBodyType);

            _turretMovement.Setup(_tankViewModel.TurretGameObject, _tankViewModel.FirePosition, tankMovementPreset);
            _cameraController.Setup(_turretMovement, transform);

            _attackController = _attackControllerFactory.GetAttackController(turretType);
            _attackController.Setup(_tankFireSettings, _tankViewModel.FirePosition.transform, _bulletFactory,
                _tankViewModel.FireRange);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && _attackController.CanFire)
            {
                _attackController.Fire();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _attackController.StopFire();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_tankViewModel != null)
            {
                _tankViewModel.DebugSetupBody(_currentBodyType);
                _tankViewModel.DebugSetupTurret(_currentTurretType);

            }
        }
        
#endif
    }
}