using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PathCreation;
using Project;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UniTaskExtensions = Project.UniTaskExtensions;

public class ReturnToPathBehaviour : EnemyBehaviourBase
{
    private Action<EnemyBehaviourType, Transform> _changeStateCallback;
    private Vector3 _closestPointOnPath;

    private PathCreator _pathCreator;
    private NavMeshAgent _agent;
    private ObservableSphereTriggerTrigger _seePlayerTrigger;
    private Transform _transform;
    private Transform _turretTransform;
    private CancellationTokenSource _rotToken;

    public ReturnToPathBehaviour(NavMeshAgent agent, PathCreator pathCreator,
        ObservableSphereTriggerTrigger seePlayerTrigger, Action<EnemyBehaviourType, Transform> changeStateCallback,
        Transform transform, Transform turretTransform)
    {
        _turretTransform = turretTransform;
        _transform = transform;
        _changeStateCallback = changeStateCallback;
        _seePlayerTrigger = seePlayerTrigger;
        _agent = agent;
        _pathCreator = pathCreator;
    }
    
    public override void Enter()
    {
        base.Enter();

        _closestPointOnPath = _pathCreator.path.GetClosestPointOnPath(_agent.transform.position);
        _agent.stoppingDistance = 0;
        _agent.SetDestination(_closestPointOnPath);

        ChangeTurretRot(UniTaskUtil.RefreshToken(ref _rotToken));

        SetupTrigger();
    }

    public override void Exit()
    {
        base.Exit();

        UniTaskUtil.CancelToken(ref _rotToken);
    }


    private void SetupTrigger()
    {
        _subscribeLinks.Add(_seePlayerTrigger.ObservableTriggerTrigger.OnTriggerEnterAsObservable().Subscribe(other =>
        {
            if (!_isActive)
            {
                return;
            }

            if (other.TryGetComponent(out TankController tankController))
            {
                _changeStateCallback(EnemyBehaviourType.MoveToPlayer, tankController.transform);
            }
        }));
    }

    public override void Tick()
    {
        var distance = Vector3.Distance(_closestPointOnPath, _transform.position);

        if (distance <= 0.4f)
        {
            _changeStateCallback(EnemyBehaviourType.FollowPath, null);
        }
    }

    private void ChangeTurretRot(CancellationToken refreshToken)
    {
        try
        {
            var start = _turretTransform.rotation;

            UniTaskExtensions.Lerp(
                time => { _turretTransform.rotation = Quaternion.Slerp(start, Quaternion.identity, time); }, 1f,
                token: refreshToken);
        }
        catch (OperationCanceledException e)
        {
        }
    }
    
    public override void Dispose()
    {
        base.Dispose();
        
        _agent.speed = 0;
    }
}