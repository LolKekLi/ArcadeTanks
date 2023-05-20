using System;
using System.Linq;
using Project;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/TankSettings", fileName = "TankSettings", order = 0)]
public class TankFireSettings : ScriptableObject
{
    public abstract class BaseFirePreset
    {
        [field: SerializeField]
        public float ReloadTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float Damage
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Bullet Bullet
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class ClassTankFirePreset : BaseFirePreset
    {
    }

    [Serializable]
    public class FireTankFirePreset : BaseFirePreset
    {
        [field: SerializeField]
        public float AttackTime
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class TwoGunAttackPreset : BaseFirePreset
    {
        [field: SerializeField]
        public float AttackTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float StartAttackTime
        {
            get;
            private set;
        }
    }


    [field: SerializeField]
    public ClassTankFirePreset ClassicFirePresets
    {
        get;
        private set;
    }

    [field: SerializeField]
    public FireTankFirePreset FireTankFirePresets
    {
        get;
        private set;
    }

    [field: SerializeField]
    public TwoGunAttackPreset TwoGunFirePreset
    {
        get;
        private set;
    }

    public Bullet GetBullet(TurretType type)
    {
        switch (type)
        {
            case TurretType.Classic:
                return ClassicFirePresets.Bullet;
                break;
            case TurretType.Fire:
                return FireTankFirePresets.Bullet;
                break;
            case TurretType.TwoGuns:
                return TwoGunFirePreset.Bullet;
                break;
            default:
                return null;
        }
    }
}