using UnityEngine;

namespace Project.Settings
{
    [CreateAssetMenu(fileName = "PoolSettings", menuName = "Scriptable/PoolSettings", order = 0)]
    public class PoolSettings : ScriptableObject
    {
        [field: SerializeField]
        public PooledAudio PooledAudio
        {
            get;
            private set;
        }
    }
}