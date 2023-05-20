using UnityEngine;

namespace Project
{
    public static class VectorExtensions
    {
        public static Vector3 ChangeX(this Vector3 origin, float value)
        {
            origin.x = value;
            return origin;
        }

        public static Vector3 ChangeY(this Vector3 origin, float value)
        {
            origin.y = value;
            return origin;
        }

        public static Vector3 ChangeZ(this Vector3 origin, float value)
        {
            origin.z = value;
            return origin;
        }

        public static Vector2 ChangeX(this Vector2 origin, float value)
        {
            origin.x = value;
            return origin;
        }

        public static Vector2 ChangeY(this Vector2 origin, float value)
        {
            origin.y = value;
            return origin;
        }
    }
}