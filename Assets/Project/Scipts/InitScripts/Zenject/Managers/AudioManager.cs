using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Project
{
    [Serializable]
    public class SoundSetup
    {
        [SerializeField]
        private SoundType _audioType;

        [SerializeField]
        private AudioClip[] _clips;

        [field: SerializeField, Range(0f, 1f)]
        public float Volume
        {
            get;
            private set;
        } = 1;

        [field: SerializeField]
        public bool IsLoop
        {
            get;
            private set;
        }

        public SoundType AudioType
        {
            get =>
                _audioType;
        }

        public AudioClip[] Clips
        {
            get =>
                _clips;
        }
    }

    public class AudioManager : ZenjectManager<AudioManager>
    {
        protected class LoopedAudioData
        {
            public PooledAudio PooledAudio;
            public CancellationTokenSource TokenSource;
        }

        private readonly ReactiveProperty<bool> _isMuted = new ReactiveProperty<bool>();

        [SerializeField]
        private SoundSetup[] _setups = null;

        [SerializeField]
        private AudioSource _audioSource = null;

        [SerializeField]
        private float _defaultSmoothChangeTime = 0.2f;

        private PoolManager _poolManager = null;
        private Dictionary<SoundType, LoopedAudioData> _loopedAudios;
        private LevelFlowController _levelFlowController;


        [Inject]
        public void Construct(PoolManager poolManager, LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
            _poolManager = poolManager;
        }

        public bool IsAudioMuted
        {
            get =>
                _isMuted.Value;

            set =>
                _isMuted.Value = value;
        }

        protected override void Init()
        {
            base.Init();
            _loopedAudios = new Dictionary<SoundType, LoopedAudioData>();

            var soundSetups = _setups.Where(x => x.IsLoop);

            foreach (var setup in soundSetups)
            {
                _loopedAudios.Add(setup.AudioType, new LoopedAudioData());
            }

            _isMuted.Value = LocalConfig.IsMuteAudio;

            _isMuted.Subscribe(isMuted =>
            {
                LocalConfig.IsMuteAudio = isMuted;

                RefreshLoopedAudio(isMuted);
            });

            ButtonExtensions.SetManagerInstance(this);
        }
        
        private void RefreshLoopedAudio(bool isMuted)
        {
            foreach (var pooledAudio in _loopedAudios)
            {
                var pooledAudioValue = pooledAudio.Value;
                if (!pooledAudioValue.PooledAudio)
                {
                    continue;
                }

                var setup = _setups.FirstOrDefault(x => x.AudioType == pooledAudio.Key);

                if (setup != null)
                {
                    pooledAudioValue.PooledAudio.Source.volume = isMuted ? 0 : setup.Volume;
                }
            }
        }

        public void Play3DSound(SoundType type, Vector3 position)
        {
            if (IsAudioMuted)
            {
                return;
            }

            var sound = GetSound(type);

            Play3D(sound, position);
        }

        public void Play2DSound(SoundType type)
        {
            if (IsAudioMuted)
            {
                return;
            }

            AudioClip clip = GetSound(type);

            if (clip)
            {
                Play2D(clip);
            }
        }

        private void Play3D(AudioClip clip, Vector3 position, float pitch = 1f, float vol = 1f)
        {
            //cancel execution if clip wasn't set
            if (clip == null)
            {
                return;
            }

            var audio = _poolManager.Get<PooledAudio>(_poolManager.PoolSettings.PooledAudio,
                position, Quaternion.identity);

            audio.Setup(clip, pitch);
        }

        private void Play2D(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
        }

        private AudioClip GetSound(SoundType type)
        {
            var setup = _setups.FirstOrDefault(s => s.AudioType == type);

            if (setup.Clips != null && setup.Clips.Length > 0)
            {
                return setup.Clips.RandomElement();
            }

            return null;
        }

        public void PlayLoopedSound(SoundType soundType, Vector3 position, bool isForcePlay,
            float changeVolumeTime = -1)
        {
            var soundSetup = _setups.FirstOrDefault(x => x.AudioType == soundType);

            if (soundSetup == null)
            {
                DebugSafe.LogError($"Not found sound preset by type {soundType}");
                return;
            }

            PooledAudio pooledAudio = null;
            var soundSetupVolume = _isMuted.Value ? 0 : soundSetup.Volume;

            if (!_loopedAudios[soundType].PooledAudio)
            {
                pooledAudio = _poolManager.Get<PooledAudio>(_poolManager.PoolSettings.PooledAudio,
                    position, Quaternion.identity);

                if (isForcePlay)
                {
                    pooledAudio.Setup(soundSetup.Clips.RandomElement(), 1, soundSetupVolume, true);
                }
                else
                {
                    pooledAudio.Setup(soundSetup.Clips.RandomElement(), 1, 0, true);

                    SmoothChangeVolume(pooledAudio.Source, soundSetupVolume,
                        changeVolumeTime == -1 ? _defaultSmoothChangeTime : changeVolumeTime,
                        UniTaskUtil.RefreshToken(ref _loopedAudios[soundType].TokenSource)).Forget();
                }
            }
            else
            {
                pooledAudio = _loopedAudios[soundType].PooledAudio;

                if (isForcePlay)
                {
                    pooledAudio.Source.volume = soundSetupVolume;
                }
                else
                {
                    SmoothChangeVolume(pooledAudio.Source, soundSetupVolume,
                        changeVolumeTime == -1 ? _defaultSmoothChangeTime : changeVolumeTime,
                        UniTaskUtil.RefreshToken(ref _loopedAudios[soundType].TokenSource)).Forget();
                }
            }

            _loopedAudios[soundType].PooledAudio = pooledAudio;
        }

        public void StopLoopedSound(SoundType soundType, bool isForceStop,
            float changeVolumeTime = -1)
        {
            if (isForceStop)
            {
                ForceStop(soundType);
            }
            else
            {
                if (!_loopedAudios[soundType].PooledAudio)
                {
                    return;
                }
                
                SmoothChangeVolume(_loopedAudios[soundType].PooledAudio.Source, 0,
                        changeVolumeTime == -1 ? _defaultSmoothChangeTime : changeVolumeTime,
                        UniTaskUtil.RefreshToken(ref _loopedAudios[soundType].TokenSource), () => ForceStop(soundType))
                    .Forget();
            }
        }

        private void ForceStop(SoundType soundType)
        {
            if (_loopedAudios.ContainsKey(soundType) && _loopedAudios[soundType].PooledAudio)
            {
                _loopedAudios[soundType].PooledAudio.Free();
                _loopedAudios[soundType].PooledAudio = null;
                UniTaskUtil.CancelToken(ref _loopedAudios[soundType].TokenSource);
            }
        }

        private async UniTaskVoid SmoothChangeVolume(AudioSource audioSource, float targetVolume,
            float executeTime, CancellationToken refreshToken, Action callBack = null)
        {
            var startVolume = audioSource.volume;
            try
            {
                await UniTaskExtensions.Lerp(
                    time => { audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time); },
                    executeTime, token: refreshToken);

                callBack?.Invoke();
            }
            catch (OperationCanceledException e)
            {
            }
        }
    }
}