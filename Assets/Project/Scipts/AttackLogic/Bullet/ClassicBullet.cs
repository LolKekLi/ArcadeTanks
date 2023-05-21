﻿using System;
using Cysharp.Threading.Tasks;
using Project;
using UnityEngine;

public class ClassicBullet : Bullet
{
    [SerializeField, Space]
    private float _speed = 10;

    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private ParticleSystem _onInteractedFX;

    private Vector3 _fireDirection;
    private float _cuuerntSpeed;
    
    
    public override void Setup(float damage)
    {
        base.Setup(damage);
        
        _cuuerntSpeed = _speed;
    }

    private void Update()
    {
        var targetPosition = transform.position + _fireDirection * (_cuuerntSpeed * Time.deltaTime);
        _rigidbody.MovePosition(targetPosition);
    }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();
        _fireDirection = Vector3.zero;
        _renderer.enabled = true;
    }

    public override void Fire(Vector3 fireDirection)
    {
        _fireDirection = fireDirection.normalized;
    }

    protected override async void OnInteracted(Collider damagable)
    {
        _cuuerntSpeed = 0;
        _renderer.enabled = false;
        _onInteractedFX.Play();
        
        base.OnInteracted(damagable);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        Free();
    }
}