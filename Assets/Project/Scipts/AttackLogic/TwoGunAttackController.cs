using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public class TwoGunAttackController : AttackControllerBase
    {
        private bool _isRight;
        private bool _isOverheat;
        private float _fireRange;
        private float _reloadedTime;
        private float _currentFireSpeed;
        private float _fireTime;
        private float _delay;

        private CancellationTokenSource _overclockingToken;
        private TankFireSettings.TwoGunAttackPreset _currentFirePreset;

        public override bool CanFire
        {
            get =>
                _fireTime + _delay <= Time.time && Time.time >= _reloadedTime;
        }

        public override TurretType Type
        {
            get =>
                TurretType.TwoGuns;
        }

        public override bool IsOverheat
        {
            get => _isOverheat;
        }

        public override void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory,
            float fireRange, Action<bool> onFireStopedCallBack, ParticleSystem onFireParticle,
            AudioManager audioManager)
        {
            base.Setup(fireSettings, firePosition, bulletFactory, fireRange, onFireStopedCallBack, onFireParticle, audioManager);
            
            _fireRange = fireRange;
            _currentFirePreset = fireSettings.TwoGunFirePreset;
        }

        public override void Fire()
        {
            if (_isOverheat)
            {
                _isOverheat = false;
            }
            
            _isRight = !_isRight;

            _fireTime = Time.time;

            if (_overclockingToken == null)
            {
                Overcloking(UniTaskUtil.RefreshToken(ref _overclockingToken));
            }

            var _bullet = _bulletFactory.GetBullet(Type);

            _bullet.transform.position = _firePosition.position +
                _firePosition.TransformDirection(new Vector3(_fireRange, 0)) * (_isRight ? 1 : -1);
            _bullet.transform.rotation = Quaternion.LookRotation(_firePosition.forward);
            _bullet.Setup(_currentFirePreset.Damage);
            _bullet.Fire(_firePosition.forward);
        }

        private async void Overcloking(CancellationToken refreshToken)
        {
            try
            {
                await UniTaskExtensions.Lerp(
                    time =>
                    {
                        _delay = Mathf.Lerp(_currentFirePreset.FireDelay.Max, _currentFirePreset.FireDelay.Min, time);
                    }, _currentFirePreset.OverclockingTime, _currentFirePreset.OverclockingCurve, token: refreshToken);

                await UniTask.Delay(TimeSpan.FromSeconds(_currentFirePreset.OverHeatTime),
                    cancellationToken: refreshToken);

                _reloadedTime = Time.time + _currentFirePreset.ReloadTime;
                _isOverheat = true;

                StopFire(true);
            }
            catch (OperationCanceledException e)
            {
            }
        }

        public override void StopFire(bool isOverhead)
        {
            _onFireStopedCallBack?.Invoke(isOverhead);

            UniTaskUtil.CancelToken(ref _overclockingToken);
            _delay = _currentFirePreset.FireDelay.Max;
        }
    }
}