using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;
using Zenject;
using UniTaskExtensions = Project.UniTaskExtensions;

public class EnemyController : MonoBehaviour, IDamagable , ITank
{

#if UNITY_EDITOR
    public bool _isActiveDebug;
#endif
    
    [SerializeField]
    private TankViewModel _tankViewModel;

    [SerializeField]
    private TurretType _turretType;
    [SerializeField]
    private BodyType _bodyType;
    
    [SerializeField, Space]
    private Collider _headColliedr;

    [SerializeField]
    private int _startHP;

    [SerializeField]
    private GameObject _hpBar;

    [SerializeField]
    private float _hpBarEnableTime;

    [SerializeField]
    private LayerMask _playerLayerMask;

    [SerializeField, Header("Move")]
    private PathCreator _pathCreator;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private EndOfPathInstruction _endOfPathInstruction;
    
    [SerializeField]
    private NavMeshAgent _agent;

    [SerializeField]
    private ObservableSphereTriggerTrigger _seePlayerTrigger;

    [SerializeField]
    private ObservableSphereTriggerTrigger _attackTrigger;
    
    private float _currentHP;
    private float _distanceTravelled;

    private Transform _hpBarTransform;
    private CanvasGroup _hpBarCanvasGroup;
    private SlicedFilledImage _hpBatSlicedFilledImage;
    private Transform _cameraControllerTransform;
    private Transform _transform;

    private CancellationTokenSource _HBBarRotationToken;
    private CancellationTokenSource _hpBarSmooThChangeRotation;
    private EnemyBehaviourFSM _fsm;

    [Inject]
    private CameraController _cameraController;

    [Inject]
    private AttackControllerFactory _attackControllerFactory;

    [Inject]
    private TankFireSettings _fireSettings;

    [Inject]
    private BulletFactory _bulletFactory;

    private Action _onDied;

    public float HP
    {
        get =>
            _currentHP;
    }

    public bool IsDied
    {
        get =>
            _currentHP <= 0;
    }
    
    public TurretType Type
    {
        get => _turretType;
    }

    private void OnDisable()
    {
        UniTaskUtil.CancelToken(ref _HBBarRotationToken);
        UniTaskUtil.CancelToken(ref _hpBarSmooThChangeRotation);
    }

    private void Start()
    {
        _currentHP = _startHP;

        _tankViewModel.Setup(_turretType, _bodyType);
        _cameraControllerTransform = _cameraController.transform;
        _hpBarTransform = _hpBar.transform;
        _hpBarCanvasGroup = _hpBar.GetComponent<CanvasGroup>();
        _hpBarCanvasGroup.alpha = 0;
        _hpBatSlicedFilledImage = _hpBar.GetComponentInChildren<SlicedFilledImage>();
        _hpBatSlicedFilledImage.fillAmount = 1;

        _transform = transform;

        var attackControllerBase = _attackControllerFactory.GetAttackController(_turretType);

        attackControllerBase.Setup(_fireSettings, _tankViewModel.FirePosition.transform, _bulletFactory,
            _tankViewModel.FireRange, null, _tankViewModel.OnFireParticle, null);

        _fsm = new EnemyBehaviourFSM(transform, _pathCreator, attackControllerBase,
            _tankViewModel.TurretGameObject.transform,
            _agent, _seePlayerTrigger, _attackTrigger, _playerLayerMask, _speed);
    }
    
    public void Setup(Action onDied)
    {
        _onDied = onDied;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_isActiveDebug)
        {
            return;
        }
#endif
        _fsm.CurrentBehaviour?.Tick();
    }

    private async UniTaskVoid UpdateHpRotation(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var direction = (_cameraControllerTransform.position - _transform.position).ChangeY(0);
                var lookRotation = Quaternion.LookRotation(direction);
                _hpBarTransform.rotation = lookRotation;
                await UniTask.Yield(token);
            }
        }
        catch (OperationCanceledException e)
        {
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            Died();
        }
        else
        {
            ChangeHpBarValue();
        }
    }

    private void ChangeHpBarValue()
    {
        _hpBatSlicedFilledImage.fillAmount = _currentHP / _startHP;
    }

    public void Died()
    {
        _onDied?.Invoke();
        _fsm.OnDied();
        DisableHpBar();
        _headColliedr.enabled = false;
        _tankViewModel.OnDied();
    }

    public void EnableHpBar()
    {
        SmoothChangeHpBarAlfa(1, UniTaskUtil.RefreshToken(ref _hpBarSmooThChangeRotation));
        UpdateHpRotation(UniTaskUtil.RefreshToken(ref _HBBarRotationToken)).Forget();
    }

    public void DisableHpBar()
    {
        SmoothChangeHpBarAlfa(0, UniTaskUtil.RefreshToken(ref _hpBarSmooThChangeRotation));
        UniTaskUtil.CancelToken(ref _HBBarRotationToken);
    }

    private void SmoothChangeHpBarAlfa(int targetValue, CancellationToken cancellationToken)
    {
        try
        {
            var start = _hpBarCanvasGroup.alpha;

            UniTaskExtensions.Lerp(time => { _hpBarCanvasGroup.alpha = Mathf.Lerp(start, targetValue, time); },
                _hpBarEnableTime, token: cancellationToken);
        }
        catch (OperationCanceledException e)
        {
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_tankViewModel != null)
        {
            _tankViewModel.DebugSetupBody(_bodyType);
            _tankViewModel.DebugSetupTurret(_turretType);
        }
    }
#endif

   
}