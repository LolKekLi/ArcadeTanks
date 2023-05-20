using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Project
{
    [Serializable]
    public struct SoundSetup
    {
        [SerializeField]
        private SoundType _audioType;

        [SerializeField]
        private AudioClip[] _clips;

        public SoundType AudioType
        {
            get => _audioType;
        }

        public AudioClip[] Clips
        {
            get => _clips;
        }
    }
    
    public class AudioManager : ZenjectManager<AudioManager>
    {
        [SerializeField]
        private SoundSetup[] _setups = null;
        
        [SerializeField]
        private AudioSource _audioSource = null;

        private PoolManager _poolManager = null;

        [Inject]
        public void Construct(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }
        
        protected override void Init()
        {
            base.Init();
            
        }

        public void Play3DSound(SoundType type, Vector3 position)
        {
            var sound = GetSound(type);

            Play3D(sound, position);
        }
        
        public void Play2DSound(SoundType type)
        {
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
    }
}