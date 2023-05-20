using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Meta
{
    [Serializable]
    public class SkinPreset
    {
        [field: SerializeField]
        public SkinType SkinType
        {
            get;
            private set;
        }

        [field: SerializeField]
        public SkinPartType SkinPartType
        {
            get;
            private set;
        }

        [field: SerializeField]
        public RarityType RarityType
        {
            get;
            private set;
        }

        [SerializeField]
        private UnlockType _unlockType = UnlockType.Locked;

        [SerializeField, EnabledIf(nameof(_unlockType), (int)UnlockType.Ads, EnabledIfAttribute.HideMode.Invisible)]
        private int _skinUnlockCount = 0;

        public UnlockType UnlockType
        {
            get => _unlockType;
        }

        public int SkinUnlockCount
        {
            get => _skinUnlockCount;
        }
    }

    [Serializable]
    public class RarityPreset
    {
        [SerializeField]
        public RarityType _rarityType = default;
        
        public RarityType RarityType
        {
            get => _rarityType;
        }
    }
    
    [CreateAssetMenu(fileName = "SkinSettings", menuName = "Scriptable/SkinSettings", order = 0)]
    public class SkinSettings : ScriptableObject
    {
        [SerializeField]
        private SkinPreset[] _skinPresets = null;
        
        public SkinType GetCurrentSkin(SkinPartType partType)
        {
            return LocalConfig.GetSelectedSkin(partType);
        }
        
        public List<SkinType> GetSkins(SkinPartType skinPartType)
        {
            List<SkinType> skins = new List<SkinType>();

            for (int i = 0; i < _skinPresets.Length; i++)
            {
                if (_skinPresets[i].SkinPartType == skinPartType)
                {
                    skins.Add(_skinPresets[i].SkinType);
                }
            }

            return skins;
        }

        public List<SkinPreset> GetSkinPresets(SkinPartType skinPartType)
        {
            List<SkinPreset> presets = new List<SkinPreset>();

            for (int i = 0; i < _skinPresets.Length; i++)
            {
                if (_skinPresets[i].SkinPartType == skinPartType)
                {
                    presets.Add(_skinPresets[i]);
                }
            }

            return presets;
        }
    }
}