using UnityEngine;

namespace Project
{
    public abstract class AttackControllerBase
    {
        protected Transform _firePosition;

        protected BulletFactory _bulletFactory;

        public abstract bool CanFire
        {
            get;
        }

        public abstract TurretType Type
        {
            get;
        }

        public virtual void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory, float fireRange)
        {
            _firePosition = firePosition;
            _bulletFactory = bulletFactory;
        }

        public abstract void Fire();
        public abstract void StopFire();
    }
}