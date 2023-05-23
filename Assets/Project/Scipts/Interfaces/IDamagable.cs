public interface IDamagable
{
    float HP
    {
        get;
    }

    bool IsDied
    {
        get;
    }

    public void TakeDamage(float damage);

    public void Died();
}