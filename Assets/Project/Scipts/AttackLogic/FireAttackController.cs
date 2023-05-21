using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public class FireAttackController : AttackControllerBase
    {
        private float _nextFireTime;
        private float _reloadedTime;
        
        private TankFireSettings.FireTankFirePreset _currentFirePreset;
        private Bullet _bullet;
        private CancellationTokenSource _flameToken;

        public override TurretType Type
        {
            get =>
                TurretType.Fire;
        }

        public override bool CanFire
        {
            get
            {
                return  Time.time >= _nextFireTime && Time.time >= _reloadedTime;
            }
        }

        public override void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory,  float fireRange)
        {
            base.Setup(fireSettings, firePosition, bulletFactory, fireRange);
            _currentFirePreset = fireSettings.FireTankFirePresets;
        }

        public override void Fire()
        {
            if (_bullet == null)
            {
                _bullet = _bulletFactory.GetBullet(Type);
                var _bulletTransform = _bullet.transform;
                _bulletTransform.position = _firePosition.position;
                _bulletTransform.rotation = Quaternion.LookRotation(_firePosition.forward);
                _bulletTransform.parent = _firePosition;
                _bullet.Setup(_currentFirePreset.Damage);

                StartFlaimTimer(UniTaskUtil.RefreshToken(ref _flameToken)).Forget();
            }

            _nextFireTime = Time.deltaTime + _currentFirePreset.FireDelay;
            _bullet.Fire(_firePosition.forward);
        }

        private async UniTaskVoid StartFlaimTimer(CancellationToken refreshToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_currentFirePreset.FlameAttackTime), cancellationToken: refreshToken);
                _reloadedTime = Time.deltaTime + _currentFirePreset.ReloadTime;
                StopFire();
            }
            catch (OperationCanceledException e)
            {
            }
        }

        public override void StopFire()
        {
            if (_bullet)
            {
                UniTaskUtil.CancelToken(ref _flameToken);
                _bullet.Free();
                _bullet = null;
            }
        }
    }
}