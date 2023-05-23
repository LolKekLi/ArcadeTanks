using System;
using Project.Meta;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TankController : MonoBehaviour, ITank, IDamagable
    {
        public static event Action Fired = delegate { }; 
        public static event Action<float> Damaged = delegate {  };

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

        private float _currentHp;
        
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
        
        public float HP
        {
            get;
        }

        public bool IsDied
        {
            get;
        }

        public TurretType Type
        {
            get =>
                _currentTurretType;
        }
        
        private void Awake()
        {
            Setup(_user.TurretType.Value, _user.BodyType.Value);
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

            _currentHp = tankMovementPreset.HP;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && _attackController.CanFire)
            {
                Fired();
                _attackController.Fire();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _attackController.StopFire();
            }

#if UNITY_EDITOR
            DebugUpdate();
#endif
        }
        
        public void TakeDamage(float damage)
        {
            _currentHp -= damage;

            Damaged(_currentHp);
            
            if (_currentHp <= 0)
            {
                Died();
            }
        }

        public void Died()
        {
            _tankViewModel.OnDied();
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
        
        private void DebugUpdate()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                TakeDamage(1);
            }
        }
#endif
      
    }
}