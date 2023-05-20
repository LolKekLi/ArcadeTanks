using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public static class UniTaskExtensions
    {
        public static async UniTask Lerp(Action<float> action, float executionTime, AnimationCurve curve = null,
            CancellationToken token = default, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.FixedUpdate)
        {
            float evaluate(float progress)
            {
                if (curve != null)
                {
                    progress = curve.Evaluate(progress);
                }

                return progress;
            }
            
            float time = 0f;
            float progress = 0f;

            while (time < executionTime)
            {
                await UniTask.Yield(playerLoopTiming, token);

                time += GetLoopUpdateTime(playerLoopTiming);
                progress = time / executionTime;
                progress = evaluate(progress);

                action(progress);
            }
        }

        private static float GetLoopUpdateTime(PlayerLoopTiming playerLoopTiming)
        {
            float deltaTime = 0f;
            
            switch (playerLoopTiming)
            {
                case PlayerLoopTiming.Update:
                    deltaTime = Time.deltaTime;
                    break;
                
                case PlayerLoopTiming.FixedUpdate:
                    deltaTime = Time.fixedDeltaTime;
                    break;
                
                default:
                    deltaTime = Time.deltaTime;
                    break;
            }

            return deltaTime;
        }
    }
}