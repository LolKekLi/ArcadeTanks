using System;
using UnityEngine;

namespace Project.Settings
{
    [CreateAssetMenu(fileName = "ParticleSettings", menuName = "Scriptable/ParticleSettings", order = 0)]
    public class ParticleSettings : ScriptableObject
    {
        [Serializable]
        public class ParticlePreset
        {
            [field: SerializeField]
            public PooledParticle Particle
            {
                get;
                private set;
            }

            [field: SerializeField]
            public ParticleType Type
            {
                get;
                private set;
            }
        }   
        
        [field: SerializeField]
        public ParticlePreset[] ParticlePresets
        {
            get;
            private set;
        }
    }
}