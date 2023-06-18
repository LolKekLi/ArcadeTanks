using UnityEngine;

public class FireBullet : Bullet
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    private IDamagable _target;

    public override void Setup(float damage, int layer)
    {
        base.Setup(damage, layer);

        _particleSystem.Play();
    }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();

        _particleSystem.Stop();
    }

    public override void Fire(Vector3 transformForward)
    {
        if (_target != null && !_target.IsDied)
        {
            _target.TakeDamage(_damage);
        }
    }

    protected override void OnInteracted(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            _target = damagable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            _target = null;
        }
    }
}