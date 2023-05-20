using UnityEngine;

namespace Project.Settings
{
    [CreateAssetMenu(fileName = "LoadingSettings", menuName = "Scriptable/LoadingSettings", order = 0)]
    public class LoadingSettings : ScriptableObject
    {
        [field: SerializeField]
        public float LoadingTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float FadeTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public AnimationCurve FadeCurve
        {
            get;
            private set;
        }
    }
}