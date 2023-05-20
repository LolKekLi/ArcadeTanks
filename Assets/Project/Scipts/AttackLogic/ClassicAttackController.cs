using UnityEngine;

namespace Project
{
    public class ClassicAttackController : AttackControllerBase
    {
        private float _canFireTime;

        private TankFireSettings.ClassTankFirePreset _currentFirePreset;

        public override bool CanFire
        {
            get =>
                Time.time >= _canFireTime;
        }

        public override TurretType Type
        {
            get =>
                TurretType.Classic;
        }

        public override void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory)
        {
            base.Setup(fireSettings, firePosition, bulletFactory);
            _currentFirePreset = fireSettings.ClassicFirePresets;
        }

        public override void Fire()
        {
            _canFireTime = Time.time + _currentFirePreset.ReloadTime;

            var _bullet = _bulletFactory.GetBullet(Type);

            _bullet.transform.position = _firePosition.position;
            _bullet.transform.rotation = Quaternion.LookRotation(_firePosition.forward);
            _bullet.Setup(_currentFirePreset.Damage);
            _bullet.Fire(_firePosition.forward);
        }

        public override void StopFire()
        {
       
        }
    }
}