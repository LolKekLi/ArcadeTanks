using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PathCreation;
using Project;
using UniRx;
using UnityEngine;

public class FollowPathBehaviour : EnemyBehaviourBase
{
    private float _speed;
    private float _distanceTravelled;

    private Action<EnemyBehaviourType, Transform> _changeStateCallback;
    private LayerMask _layerMask;
    private EndOfPathInstruction _endOfPathInstruction;

    private Transform _transform;
    private PathCreator _pathCreator;
    private ObservableSphereTriggerTrigger _seePlayerTrigger;
    private CancellationTokenSource _chekcToken;


    public FollowPathBehaviour(Transform transform, PathCreator pathCreator,
        ObservableSphereTriggerTrigger seePlayerTrigger,
        Action<EnemyBehaviourType, Transform> changeStateCallback, LayerMask layerMask, float speed)
    {
        _speed = speed;
        _layerMask = layerMask;
        _changeStateCallback = changeStateCallback;
        _seePlayerTrigger = seePlayerTrigger;
        _transform = transform;
        _pathCreator = pathCreator;
    }

    public override void Enter()
    {
        base.Enter();

        _distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(_transform.position);

        SetupTriggers();
    }

    public override void Exit()
    {
        base.Exit();

        UniTaskUtil.CancelToken(ref _chekcToken);
    }

    private void SetupTriggers()
    {
        var link = _seePlayerTrigger.ObservableTriggerTrigger.OnTriggerEnterAsObservable().Subscribe(other =>
        {
            if (other.TryGetComponent(out TankController tankController))
            {
                CheckWallAsync(UniTaskUtil.RefreshToken(ref _chekcToken), tankController);
            }
        });

        _subscribeLinks.Add(link);

        _seePlayerTrigger.ObservableTriggerTrigger.OnTriggerExitAsObservable().Subscribe(other =>
        {
            if (other.TryGetComponent(out TankController tankController))
            {
                UniTaskUtil.CancelToken(ref _chekcToken);
            }
        });

        _subscribeLinks.Add(link);
    }

    private async void CheckWallAsync(CancellationToken refreshToken, TankController tankController)
    {
        try
        {
            while (!refreshToken.IsCancellationRequested)
            {
                var position = _transform.position;
                var direction = (tankController.transform.position - position).normalized;

                Ray ray = new Ray(position, direction * 10000);
                Debug.DrawRay(position, direction * 10000);

                if (Physics.Raycast(ray, out RaycastHit _hit))
                {
                    var hitInfo = _hit.transform.GetComponent<TankController>();

                    if (hitInfo != null)
                    {
                        _changeStateCallback(EnemyBehaviourType.MoveToPlayer, tankController.transform);
                    }
                }

                await UniTask.Yield(cancellationToken: refreshToken);
            }
        }
        catch (OperationCanceledException e)
        {
        }
    }

    public override void Tick()
    {
        _distanceTravelled += _speed * Time.deltaTime;
        _transform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction);
        _transform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled, _endOfPathInstruction);
    }
}