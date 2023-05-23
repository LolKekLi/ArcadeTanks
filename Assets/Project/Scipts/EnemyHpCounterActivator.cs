public class EnemyHpCounterActivator : InteractableObject<EnemyController>
{
    protected override void OnInteracted(EnemyController component)
    {
        component.EnableHpBar();
    }

    protected override void OnExited(EnemyController component)
    {
        component.DisableHpBar();
    }
}
