using Project;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public abstract class Bullet : PooledBehaviour
{
    private float _damage;
    protected Rigidbody _rigidbody;
    private Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        OnInteracted(other);
    }

    protected virtual void OnInteracted(Collider other)
    {
        _collider.enabled = false;
        
        if (other.TryGetComponent(out IDamagable damagable))
        {
            if (!damagable.IsDied)
            {
                damagable.TakeDamage(_damage);
            }
        }
    }

    public override void SpawnFromPool()
    {
        base.SpawnFromPool();

        _collider.enabled = true;
    }

    public override void Prepare(PooledObjectType pooledType)
    {
        base.Prepare(pooledType);
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public virtual void Setup(float damage)
    {
        _damage = damage;
    }

    private void GetDamage(IDamagable damagable)
    {
        damagable.TakeDamage(_damage);
    }

    public abstract void Fire(Vector3 transformForward);
}