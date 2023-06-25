public interface IDamagable
{
    bool IsDied
    {
        get;
    }

    public void TakeDamage(float damage);
}