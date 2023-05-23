namespace Project
 {
     public class AttackControllerFactory
     {
         public AttackControllerBase GetAttackController(TurretType type)
         {
             return type switch
             {
                 TurretType.Classic => new ClassicAttackController(),
                 TurretType.TwoGuns => new TwoGunAttackController(),
                 TurretType.Fire => new FireAttackController(),
                 _ => null,
             };
         }
     }
 }