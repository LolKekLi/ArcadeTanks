using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public class TwoGunAttackController : AttackControllerBase
    {
        private float _currentFireSpeed;
        private TankFireSettings.TwoGunAttackPreset _currentFirePreset;
        private float _fireTime;
        private float _delay;

        private CancellationTokenSource _overclockingToken;
        private bool _isRight;
        private float _fireRange;

        public override bool CanFire
        {
            get =>
                _fireTime + _delay <= Time.time;
        }

        public override TurretType Type
        {
            get =>
                TurretType.TwoGuns;
        }

        public override void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory,
            float fireRange)
        {
            _fireRange = fireRange;
            base.Setup(fireSettings, firePosition, bulletFactory, fireRange);
            _currentFirePreset = fireSettings.TwoGunFirePreset;
        }

        public override void Fire()
        {
            _isRight = !_isRight;

            _fireTime = Time.time;

            if (_overclockingToken == null)
            {
                Overcloking(UniTaskUtil.RefreshToken(ref _overclockingToken));
            }

            var _bullet = _bulletFactory.GetBullet(Type);

            _bullet.transform.position = _firePosition.position +
                _firePosition.TransformDirection(new Vector3(_fireRange, 0)) * (_isRight ? 1 : -1);
            ;
            _bullet.transform.rotation = Quaternion.LookRotation(_firePosition.forward);
            _bullet.Setup(_currentFirePreset.Damage);
            _bullet.Fire(_firePosition.forward);
        }

        private void Overcloking(CancellationToken refreshToken)
        {
            UniTaskExtensions.Lerp(
                time =>
                {
                    _delay = Mathf.Lerp(_currentFirePreset.FireDelay.Max, _currentFirePreset.FireDelay.Min, time);
                }, _currentFirePreset.OverclockingTime, token: refreshToken);
        }

        public override void StopFire()
        {
            UniTaskUtil.CancelToken(ref _overclockingToken);
            _delay = _currentFirePreset.FireDelay.Max;
        }
    }
}