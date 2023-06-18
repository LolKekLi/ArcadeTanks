using System;
using UnityEngine;

namespace Project
{
    public abstract class AttackControllerBase
    {
        protected SoundType _fireSoundType;

        protected Action<bool> _onFireStopedCallBack;

        protected Transform _firePosition;
        protected BulletFactory _bulletFactory;
        protected ParticleSystem _onFireParticle;
        protected AudioManager _audioManager;
        protected SoundType _reloadeSoundType;
        protected int _bulletLayer;

        public abstract bool CanFire
        {
            get;
        }

        public abstract TurretType Type
        {
            get;
        }

        public abstract bool IsOverheat
        {
            get;
        }

        public virtual void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory,
            float fireRange, Action<bool> onFireStopedCallBack, ParticleSystem onFireParticle,
            AudioManager audioManager, int layer)
        {
            _bulletLayer = layer;
            _audioManager = audioManager;
            _onFireParticle = onFireParticle;
            _onFireStopedCallBack = onFireStopedCallBack;
            _firePosition = firePosition;
            _bulletFactory = bulletFactory;

            _fireSoundType = GetFireSoundType();
            _reloadeSoundType = GetReloadeSoundType();
        }

        private SoundType GetFireSoundType()
        {
            return Type switch
            {
                TurretType.Classic => SoundType.ClassicFire,
                TurretType.Fire => SoundType.FlameFire,
                TurretType.TwoGuns => SoundType.TwoGunsFire,
            };
        }
        
        private SoundType GetReloadeSoundType()
        {
            return Type switch
            {
                TurretType.Classic => SoundType.Repair,
                TurretType.Fire => SoundType.RepairLoop,
                TurretType.TwoGuns => SoundType.RepairLoop,
            };
        }

        public abstract void Fire();
        public abstract void StopFire(bool isOverhead = false);

        public abstract void Dispose();
    }
}