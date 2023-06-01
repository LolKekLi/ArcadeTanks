using System;
using Project;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPlayerBehaviour : EnemyBehaviourBase
{
    private Action<EnemyBehaviourType, Transform> _changeStateCallback;

    private NavMeshAgent _agent;
    private ObservableSphereTriggerTrigger _seePlayerTrigger;
    private ObservableSphereTriggerTrigger _attackTrigger;
    private Transform _turretTransform;

    public MoveToPlayerBehaviour(NavMeshAgent agent, ObservableSphereTriggerTrigger seePlayerTrigger,
        ObservableSphereTriggerTrigger attackTrigger,
        Action<EnemyBehaviourType, Transform> changeStateCallback, Transform turretTransform)
    {
        _turretTransform = turretTransform;
        _attackTrigger = attackTrigger;
        _changeStateCallback = changeStateCallback;
        _seePlayerTrigger = seePlayerTrigger;
        _agent = agent;
    }
    
    public override void Enter()
    {
        base.Enter();

        _agent.speed = 5;
        _agent.stoppingDistance = _attackTrigger.SphereCollider.radius;

        SetupTriggers();
    }
    
    public override void Dispose()
    {
        base.Dispose();
    
        _agent.speed = 0;
    }

    private void SetupTriggers()
    {
        _seePlayerTrigger.ObservableTriggerTrigger.OnTriggerExitAsObservable().Subscribe(other =>
        {
            if (!_isActive)
            {
                return;
            }

            if (other.TryGetComponent(out TankController tankController))
            {
                _changeStateCallback(EnemyBehaviourType.ReturnToPath, null);
            }
        });

        _attackTrigger.ObservableTriggerTrigger.OnTriggerEnterAsObservable().Subscribe(other =>
        {
            if (!_isActive)
            {
                return;
            }

            if (other.TryGetComponent(out TankController tankController))
            {
                _changeStateCallback(EnemyBehaviourType.Attack, tankController.transform);
            }
        });
    }

    public override void Tick()
    {
        var position = _targetTransform.position;

        _turretTransform.LookAt(position.ChangeY(_turretTransform.transform.position.y));
        _agent.SetDestination(position);
    }

    
}