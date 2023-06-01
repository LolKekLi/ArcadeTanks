using System;
using System.Collections.Generic;
using Project;
using UniRx;
using UnityEngine;

public class AttackPlayerBehaviour : EnemyBehaviourBase
{
    private bool _isFiring;
    
    private Action<EnemyBehaviourType, Transform> _changeStateCallback;
    
    private AttackControllerBase _attackController;
    private ObservableSphereTriggerTrigger _attackTrigger;
    private Transform _turretTransform;
    private IDamagable _target;
    

    public AttackPlayerBehaviour(Transform turretTransform, AttackControllerBase attackControllerBase,
        ObservableSphereTriggerTrigger attackTrigger, Action<EnemyBehaviourType, Transform> changeStateCallback)
    {
        _turretTransform = turretTransform;
        _changeStateCallback = changeStateCallback;
        _attackTrigger = attackTrigger;
        _attackController = attackControllerBase;

    }

    public override void Enter()
    {
        base.Enter();
        
        SetupTrigger();
    }
    
    private void SetupTrigger()
    {
        var link = _attackTrigger.ObservableTriggerTrigger.OnTriggerExitAsObservable().Subscribe(other =>
        {
            if (other.TryGetComponent(out TankController tankController))
            {
                _changeStateCallback(EnemyBehaviourType.MoveToPlayer, tankController.transform);
            }
        });
        
        _subscribeLinks.Add(link);
    }

    public override void Tick()
    {
        _turretTransform.LookAt(_targetTransform.position.ChangeY(_turretTransform.transform.position.y));

        if (_target.IsDied)
        {
            _changeStateCallback(EnemyBehaviourType.ReturnToPath, null);
        }
        
        if (_attackController.CanFire)
        {
            _isFiring = true;
            _attackController.Fire();
        }
        else if (_attackController.IsOverheat)
        {
            _isFiring = false;
            _attackController.StopFire();
        }
    }

    public override void SetupTargetTransform(Transform transform)
    {
        base.SetupTargetTransform(transform);

        _target = transform.GetComponent<IDamagable>();
    }
    
}