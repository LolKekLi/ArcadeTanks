using System;
using System.Collections;
using UnityEngine;

namespace Project
{
    public static class GameObjectExtensions
    {
        public static Coroutine InvokeWithDelay(this MonoBehaviour self, float delay, Action action)
        {
            if (!self.enabled)
            {
                Debug.LogException(new Exception("gameobject is disabled"));
                return null;
            }

            return self.StartCoroutine(InvokeWithDelayCor(self, delay, action));
        }

        public static Coroutine InvokeWithFrameDelay(this MonoBehaviour self, Action action)
        {
            if (!self.enabled)
            {
                Debug.LogException(new Exception("gameobject is disabled"));
                return null;
            }

            return self.StartCoroutine(InvokeWithFrameDelayCor(self, action));
        }

        private static IEnumerator InvokeWithDelayCor(MonoBehaviour self, float delay, Action method)
        {
            yield return new WaitForSeconds(delay);
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithDelay] - exception on action : name: {0}, method: {1}, target: {2}; ex: {3}",
                    self?.gameObject?.name, method?.Method?.ToString(), method?.Target?.GetType().ToString(), ex.Message);

                throw;
            }
        }

        private static IEnumerator InvokeWithFrameDelayCor(MonoBehaviour self, Action action)
        {
            yield return null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithDelay] - exception on action : name: {0}, method: {1}, target: {2}; ex: {3}",
                    self?.gameObject?.name, action?.Method?.ToString(), action?.Target?.GetType().ToString(), ex.Message);

                throw;
            }
        }
    }
}