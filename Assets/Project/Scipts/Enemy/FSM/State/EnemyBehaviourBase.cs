using System;
using System.Collections.Generic;
using Project;
using UnityEngine;

public abstract class EnemyBehaviourBase
{
    protected bool _isActive;
    protected Transform _targetTransform;

    protected List<IDisposable> _subscribeLinks = new List<IDisposable>();

    
    public abstract void Tick();

    public virtual void Enter()
    {
        _isActive = true;
    }

    public virtual void Exit()
    {
        _isActive = false;
        
        Dispose();
    }

    public virtual void SetupTargetTransform(Transform transform)
    {
        _targetTransform = transform;
    }

    public virtual void Dispose()
    {
        _subscribeLinks.Do(x=>x.Dispose());
        _subscribeLinks.Clear();
    }
}