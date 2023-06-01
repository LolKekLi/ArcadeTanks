using System;
using System.Collections.Generic;
using Project.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

namespace Project
{
    public class TankController : MonoBehaviour, ITank, IDamagable, IEffectTarget
    {
        public static event Action Fired = delegate { };
        public static event Action<float> HpChanged = delegate { };
        public static event Action<bool> StopFire = delegate { };

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

        [SerializeField, Header("Armor")]
        private Collider _armorCollider;

        [SerializeField]
        private VisualEffect _shiedlEffect;

        private bool _isMoveSoundPlay;
        private float _maxHP;

        private ReactiveProperty<float> _currentHp = new ReactiveProperty<float>();

        private AttackControllerBase _attackController;
        private List<IDisposable> _subscribeLinks = new List<IDisposable>();

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

        [Inject]
        private LevelFlowController _levelFlowController;

        [Inject]
        private AudioManager _audioManager;

        private InputController inputs;


        public float HP
        {
            get =>
                _currentHp.Value;
        }

        public bool IsDied
        {
            get =>
                _currentHp.Value <= 0;
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

        private void OnEnable()
        {
            _levelFlowController.Finished += LevelFlowController_Finished;
        }

        private void OnDisable()
        {
            _levelFlowController.Finished -= LevelFlowController_Finished;

            _subscribeLinks.Do(x => x.Dispose());
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
                _tankViewModel.FireRange, isOverheat => { StopFire(isOverheat); }, _tankViewModel.OnFireParticle, _audioManager);

            _maxHP = tankMovementPreset.HP;
            _currentHp.Value = tankMovementPreset.HP;

            ((IEffectTarget)this).DisableArmor();

            var disposable = _currentHp.Subscribe(value => { HpChanged(_currentHp.Value); });

            _subscribeLinks.Add(disposable);
            
            inputs = GetComponent<InputController>();
        }

        private void Update()
        {
            if (IsDied)
            {
                return;
            }

            PlaySounds();

            Fire();

#if UNITY_EDITOR
            DebugUpdate();
#endif
        }

        private void PlaySounds()
        {
            // if (_tankMovement.VelocityInKMH >= 0.1 && !_isMoveSoundPlay)
            // {
            //     _isMoveSoundPlay = true;
            //     _audioManager.PlayLoopedSound(SoundType.TankMove, Vector3.zero, false);
            // }
            // else if (_isMoveSoundPlay)
            // {
            //     _isMoveSoundPlay = false;
            //     _audioManager.StopLoopedSound(SoundType.TankMove, false);
            // }
            
        }

        private void Fire()
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
        }

        public void TakeDamage(float damage)
        {
            if (_armorCollider.enabled)
            {
                return;
            }

            _currentHp.Value -= damage;

            if (_currentHp.Value <= 0)
            {
                Died();
            }
        }

        private void LevelFlowController_Finished(bool isSuccess)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            DisableControll();
        }

        public void Died()
        {
            _levelFlowController.Fail();
            _tankViewModel.OnDied();
        }

        private void DisableControll()
        {
            inputs.Free();
            inputs.enabled = false;
         
           // _tankMovement.enabled = false;
            _cameraController.enabled = false;
            _turretMovement.enabled = false;
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

        void IEffectTarget.EnableArmor()
        {
            _armorCollider.enabled = true;
            _shiedlEffect.gameObject.SetActive(true);
            _shiedlEffect.Play();
        }

        void IEffectTarget.DisableArmor()
        {
            _armorCollider.enabled = false;
            _shiedlEffect.gameObject.SetActive(false);
        }

        void IEffectTarget.ChangeSpeed(float presetValue)
        {
            _tankMovement.ChangeSpeed(presetValue);
        }

        void IEffectTarget.AddHP(float presetValue)
        {
            _currentHp.Value = Mathf.Clamp(_currentHp.Value + presetValue, 0, _maxHP);
        }
    }
}