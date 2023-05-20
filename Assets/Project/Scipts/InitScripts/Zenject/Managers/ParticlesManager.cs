using System;
using System.Collections.Generic;
using Project.Settings;
using UnityEngine;
using Zenject;

namespace Project
{
    public class ParticlesManager : ZenjectManager<ParticlesManager>
    {
        private readonly Dictionary<ParticleType, PooledParticle> _particles =
            new Dictionary<ParticleType, PooledParticle>();

        private ParticleSettings _particleSettings = null;
        
        [Inject]
        private void Construct(ParticleSettings particleSettings)
        {
            _particleSettings = particleSettings;
        }
        
        protected override void Init()
        {
            base.Init();

            for (int i = 0; i < _particleSettings.ParticlePresets.Length; i++)
            {
                var preset = _particleSettings.ParticlePresets[i];
                
                Prepare(preset.Particle, preset.Type);
            }
        }

        private void Prepare(PooledParticle pooledParticle, ParticleType fxType)
        {
            if (!_particles.ContainsKey(fxType))
            {
                _particles.Add(fxType, Instantiate(pooledParticle, Vector3.zero, Quaternion.identity, transform));
            }
            else
            {
                DebugSafe.LogException(new Exception($"{nameof(ParticleType)} already exist in dictionary"));
            }
        }

        public void Emit(ParticleType fxType, Vector3 position)
        {
            if (_particles.TryGetValue(fxType, out var particle))
            {
                particle.Emit(position);
            }
            else
            {
                DebugSafe.LogError($"Not found particle for {nameof(ParticleType)}: {fxType}");
            }
        }
    }
}