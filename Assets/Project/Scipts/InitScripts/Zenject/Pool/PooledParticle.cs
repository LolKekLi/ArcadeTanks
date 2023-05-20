using System;
using System.Collections;
using UnityEngine;

namespace Project
{
    public class PooledParticle : PooledBehaviour
    {
        private ParticleSystem[] _particleSystems = null;

        private void Awake()
        {
            _particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();

            foreach (var ps in _particleSystems)
            {
                var main = ps.main;
                if (main.scalingMode != ParticleSystemScalingMode.Hierarchy)
                {
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    Debug.LogError($"[{nameof(ParticlesManager)}] prefab:{gameObject.name}; particles system: {ps.gameObject.name}. Must be scaled mode {nameof(ParticleSystemScalingMode.Hierarchy)}");
                }

                if (main.simulationSpace != ParticleSystemSimulationSpace.World)
                {
                    main.simulationSpace = ParticleSystemSimulationSpace.World;
                    Debug.LogError($"[{nameof(ParticlesManager)}] prefab:{gameObject.name}; particle system: {ps.gameObject.name}. Must be in sumulation space {nameof(ParticleSystemSimulationSpace.World)}");
                }

                ps.Stop();
            }
        }

        public new void SpawnFromPool()
        {
            IsFree = true;
        }

        public new void Free()
        {

        }

        public void Emit(Vector3 targetPosition, int forceCount = 0)
        {
            var bursts = new ParticleSystem.Burst[10];

            foreach (var particleSystem in _particleSystems)
            {
                void emitAction(int count)
                {
                    particleSystem.Emit(GetEmitParams(particleSystem, targetPosition - transform.position + particleSystem.transform.position), count);
                }

                if (forceCount > 0)
                {
                    emitAction(forceCount);
                }
                else
                {
                    var burstCount = particleSystem.emission.GetBursts(bursts);
                    for (var i = 0; i < burstCount; i++)
                    {
                        StartCoroutine(BurstEmitCor(bursts[i], emitAction));
                    }
                }
            }
        }

        private IEnumerator BurstEmitCor(ParticleSystem.Burst burst, Action<int> emitAction)
        {
            yield return new WaitForSeconds(burst.time);

            var intervalWaiter = new WaitForSeconds(burst.repeatInterval);
            for (var cycleIndex = 0; cycleIndex < burst.cycleCount; cycleIndex++)
            {
                emitAction?.Invoke(burst.maxCount);
                yield return intervalWaiter;
            }
        }

        private ParticleSystem.EmitParams GetEmitParams(ParticleSystem particleSystem, Vector3 targetPosition)
        {
            var emitParams = new ParticleSystem.EmitParams
            {
                position = ConvertWorldPointToSimulationSpace(particleSystem, targetPosition),
                applyShapeToPosition = true
            };
            return emitParams;
        }

        private static Vector3 ConvertWorldPointToSimulationSpace(ParticleSystem particleSystem, Vector3 worldPos)
        {
            var mainModule = particleSystem.main;
            switch (mainModule.simulationSpace)
            {
                case ParticleSystemSimulationSpace.World:
                    return worldPos;

                case ParticleSystemSimulationSpace.Custom:
                    return mainModule.customSimulationSpace.InverseTransformPoint(worldPos);

                default:
                case ParticleSystemSimulationSpace.Local:
                    return particleSystem.transform.InverseTransformPoint(worldPos);
            }
        }
    }
}