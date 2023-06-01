using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Project
{
    [RequireComponent(typeof(IEffectTarget))]
    public class BusterEffectController : MonoBehaviour
    {
        private IEffectTarget _target;

        private CancellationTokenSource _armorEffectToken;
        private CancellationTokenSource _speedEffectToken;

        [Inject]
        private BusterSettings _busterSettings;

        private void Awake()
        {
            _target = GetComponent<IEffectTarget>();
        }

        private void OnDisable()
        {
            UniTaskUtil.CancelToken(ref _armorEffectToken);
            UniTaskUtil.CancelToken(ref _speedEffectToken);
        }

        public void ApplyEffect(EffectType type)
        {
            var preset = _busterSettings.GetBusterPreset(type);

            switch (type)
            {
                case EffectType.Armor:
                    ApplyEffectAsync(
                        () =>
                        {
                            _target.EnableArmor();
                        }, 
                        () =>
                        {
                            _target.DisableArmor();
                        },
                        preset.EffectTime, UniTaskUtil.RefreshToken(ref _armorEffectToken));
                    break;
                
                case EffectType.Speed:
                
                    ApplyEffectAsync(() =>
                    {
                        _target.ChangeSpeed(preset.Value);
                    }, () =>
                    {
                        _target.ChangeSpeed(0);
                    }, preset.EffectTime, UniTaskUtil.RefreshToken(ref _speedEffectToken));
                    break;
                
                
                case EffectType.Health:
                    _target.AddHP(preset.Value);
                    break;
            }
        }

        private async void ApplyEffectAsync(Action onStart, Action onEnd, float effectTime, CancellationToken token)
        {
            try
            {
                onStart?.Invoke();
                await UniTask.Delay(TimeSpan.FromSeconds(effectTime),
                    cancellationToken: token);
                onEnd?.Invoke();
            }
            catch (OperationCanceledException e)
            {
            }
        }
    }
}