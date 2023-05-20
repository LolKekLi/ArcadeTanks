namespace Project
{
    public class AttackControllerFactory
    {
        public AttackControllerBase GetAttackController(TurretType type)
        {
            return type switch
            {
                TurretType.Classic => new ClassicAttackController(),
                TurretType.TwoGuns => new ClassicAttackController(),
                TurretType.Fire => new ClassicAttackController(),
                _ => null,
            };
        }
    }
}