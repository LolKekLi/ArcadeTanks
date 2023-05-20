using UnityEngine;

namespace Project
{
    public static class FloatExtensions
    {
        public static bool AlmostEquals(this float target, float value)
        {
            return Mathf.Abs(target - value) < Mathf.Epsilon;
        }
    }
}