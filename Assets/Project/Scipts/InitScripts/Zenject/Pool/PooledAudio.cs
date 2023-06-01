using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudio : PooledBehaviour
    {
        [SerializeField]
        private AudioSource _source = null;

        public AudioSource Source
        {
            get =>
                _source;
        }

        public void Setup(AudioClip clip, float pitch, float volume = 1, bool isLooped = false)
        {
            _source.volume = volume;
            _source.clip = clip;
            _source.pitch = pitch;

            if (isLooped)
            {
                _source.loop = true;
                FreeTimeout = 0;
            }
            else
            {
                _source.loop = false;
                FreeTimeout = clip.length;
            }

            _source.Play();
        }
    }
}