using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project;
using UnityEngine;
using Zenject;
using UniTaskExtensions = Project.UniTaskExtensions;

public class EnemyController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private TankViewModel _tankViewModel;

    [SerializeField]
    private Collider _headColliedr;

    [SerializeField]
    private int _startHP;

    [SerializeField]
    private GameObject _hpBar;

    [SerializeField]
    private float _hpBarEnableTime;

    [Inject]
    private CameraController _cameraController;

    private float _currentHP;

    private Transform _hpBarTransform;
    private CanvasGroup _hpBarCanvasGroup;
    private SlicedFilledImage _hpBatSlicedFilledImage;
    private Transform _cameraControllerTransform;
    private Transform _transform;

    private CancellationTokenSource _HBBarRotationToken;
    private CancellationTokenSource _hpBarSmooThChangeRotation;

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

    private void OnDisable()
    {
        UniTaskUtil.CancelToken(ref _HBBarRotationToken);
        UniTaskUtil.CancelToken(ref _hpBarSmooThChangeRotation);
    }

    private void Start()
    {
        _currentHP = _startHP;

        _tankViewModel.Setup(TurretType.Classic, BodyType.Classic);
        _cameraControllerTransform = _cameraController.transform;
        _hpBarTransform = _hpBar.transform;
        _hpBarCanvasGroup = _hpBar.GetComponent<CanvasGroup>();
        _hpBarCanvasGroup.alpha = 0;
        _hpBatSlicedFilledImage = _hpBar.GetComponentInChildren<SlicedFilledImage>();
        _hpBatSlicedFilledImage.fillAmount = 1;

        _transform = transform;
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
        _hpBatSlicedFilledImage.fillAmount = _currentHP / (float)_startHP;
    }

    public void Died()
    {
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
}