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
            get => _source;
        }

        public void Setup(AudioClip clip, float pitch)
        {
            _source.clip = clip;
            _source.pitch = pitch;
            FreeTimeout = clip.length;
            _source.Play();
        }
    }
}