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
        private bool _isOverheat;

        private TankFireSettings.FireTankFirePreset _currentFirePreset;
        private Bullet _bullet;
        private CancellationTokenSource _flameToken;

        public override TurretType Type
        {
            get =>
                TurretType.Fire;
        }

        public override bool IsOverheat
        {
            get =>
                _isOverheat;
        }

        public override bool CanFire
        {
            get
            {
                return Time.time >= _nextFireTime && Time.time >= _reloadedTime;
            }
        }

        public override void Setup(TankFireSettings fireSettings, Transform firePosition, BulletFactory bulletFactory,
            float fireRange, Action<bool> onFireStopedCallBack, ParticleSystem onFireParticle,
            AudioManager audioManager)
        {
            base.Setup(fireSettings, firePosition, bulletFactory, fireRange, onFireStopedCallBack, onFireParticle, audioManager);
            _currentFirePreset = fireSettings.FireTankFirePresets;
        }

        public override void Fire()
        {
            if (_isOverheat)
            {
                _isOverheat = false;
            }
            
            if (_bullet == null)
            {
                _bullet = _bulletFactory.GetBullet(Type);
                var _bulletTransform = _bullet.transform;
                _bulletTransform.position = _firePosition.position;
                _bulletTransform.rotation = Quaternion.LookRotation(_firePosition.forward);
                _bulletTransform.parent = _firePosition;
                _bullet.Setup(_currentFirePreset.Damage);

                StartFlaimTimer(UniTaskUtil.RefreshToken(ref _flameToken)).Forget();
                
                _audioManager.PlayLoopedSound(_fireSoundType, Vector3.zero, false);
            }

            _nextFireTime = Time.deltaTime + _currentFirePreset.FireDelay;
            _bullet.Fire(_firePosition.forward);
        }

        private async UniTaskVoid StartFlaimTimer(CancellationToken refreshToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_currentFirePreset.FlameAttackTime),
                    cancellationToken: refreshToken);

                _reloadedTime = Time.time + _currentFirePreset.ReloadTime;
                _isOverheat = true;

                _audioManager.Play2DSound(SoundType.Overheat);
                
                StopFire(true);
                
            }
            catch (OperationCanceledException e)
            {
            }
        }

        public override void StopFire(bool isOverhead = false)
        {
            if (_bullet)
            {
                _audioManager.StopLoopedSound(_fireSoundType, false);
                
                _onFireStopedCallBack?.Invoke(isOverhead);

                UniTaskUtil.CancelToken(ref _flameToken);
                _bullet.Free();
                _bullet = null;
            }
        }

        public override void Dispose()
        {
            _bullet.Free();
            _audioManager.StopLoopedSound(_fireSoundType, false);
            UniTaskUtil.CancelToken(ref _flameToken);
        }
    }
}