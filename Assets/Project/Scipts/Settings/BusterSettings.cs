using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "Scriptable/BusterSettings", fileName = "BusterSettings", order = 0)]
    public class BusterSettings : ScriptableObject
    {
        [Serializable]
        public class BusterPreset
        {
            [field: SerializeField]
            public EffectType Type
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float Value
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float EffectTime
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private BusterPreset[] _busterPresets;

        public BusterPreset GetBusterPreset(EffectType busterType)
        {
            var preset = _busterPresets.FirstOrDefault(x=>x.Type == busterType);

            if (preset == null)
            {
                Debug.LogError($"Нет пресета под тип {busterType}");
            }

            return preset;
        }
    }
}