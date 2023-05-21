using Project;
using UnityEngine;

public class FireBullet : Bullet
{
    [SerializeField]
    private ParticleSystem _particleSystem;
    
    public override void Setup(float damage)
    {
        base.Setup(damage);
        
        _particleSystem.Play();
    }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();
        
        _particleSystem.Stop();
    }

    public override void Fire(Vector3 transformForward)
    {
       
    }
}