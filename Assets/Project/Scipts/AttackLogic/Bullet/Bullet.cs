using Project;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public abstract class Bullet : PooledBehaviour
{
    private float _damage;
    protected Rigidbody _rigidbody;

    public abstract TurretType TurretType
    {
        get;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnInteracted(other);
    }

    protected virtual void OnInteracted(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(_damage);
        }
        
    }

    public override void Prepare(PooledObjectType pooledType)
    {
        base.Prepare(pooledType);
        _rigidbody = GetComponent<Rigidbody>();
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