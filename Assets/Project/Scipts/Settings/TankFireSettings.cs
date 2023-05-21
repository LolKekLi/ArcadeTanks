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
        public float FlameAttackTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float FireDelay
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class TwoGunAttackPreset : BaseFirePreset
    {
        [field: SerializeField]
        public FloatRange FireDelay
        {
            get;
            private set;
        }
        
        [field: SerializeField]
        public float OverclockingTime
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
            case TurretType.Fire:
                return FireTankFirePresets.Bullet;
            case TurretType.TwoGuns:
                return TwoGunFirePreset.Bullet;
            default:
                return null;
        }
    }
}