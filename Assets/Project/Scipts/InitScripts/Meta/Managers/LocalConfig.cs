using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    public static class LocalConfig
    {
        private class Keys
        {
            public const string UnlockedSkin = "UnlockedSkin{0}_{1}";
            public const string SkinClaimProgress = "SkinClaimProgress{0}_{1}";
            public const string SelectedSkin = "SelectedSkin_{0}";
        }
        
        public static int GetSkinClaimProgress(SkinType skinType, SkinPartType partType)
        {
            return PlayerPrefs.GetInt(string.Format(Keys.SkinClaimProgress, skinType, partType), 0);
        } 
        
        public static void SetSkinClaimProgress(SkinType skinType, SkinPartType partType)
        {
            PlayerPrefs.SetInt(string.Format(Keys.SkinClaimProgress, skinType, partType),
                GetSkinClaimProgress(skinType, partType) + 1);
        } 

        public static bool IsSkinUnlocked(SkinType skinType, SkinPartType partType)
        {
            return GetBoolValue(string.Format(Keys.UnlockedSkin, skinType, partType), false);
        }

        public static void UnlockSkin(SkinType skinType, SkinPartType partType, bool isUnlock)
        {
            SetBoolValue(string.Format(Keys.UnlockedSkin, skinType, partType), isUnlock);
        }
        
        public static bool IsSkinSelected(SkinType skinType, SkinPartType partType)
        {
            var skins = (SkinType[]) Enum.GetValues(typeof(SkinType)); 
            var defaultSkin = skins.FirstOrDefault();

            var skin = (SkinType)Enum.Parse(typeof(SkinType),
                PlayerPrefs.GetString(string.Format(Keys.SelectedSkin, partType), defaultSkin.ToString()));
            
            return skin == skinType;
        }

        public static void SelectSkin(SkinType skinType, SkinPartType partType)
        {
            PlayerPrefs.SetString(string.Format(Keys.SelectedSkin, partType), skinType.ToString());
        }

        public static SkinType GetSelectedSkin(SkinPartType partType)
        {
            var skins = (SkinType[]) Enum.GetValues(typeof(SkinType)); 
            var defaultSkin = skins.FirstOrDefault();

            var skin = (SkinType)Enum.Parse(typeof(SkinType),
                PlayerPrefs.GetString(string.Format(Keys.SelectedSkin, partType), defaultSkin.ToString()));

            return skin;
        }

        private static void SetBoolValue(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        private static bool GetBoolValue(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
        
        private static DateTime GetDateTimeValue(string key, DateTime defaultValue)
        {
            DateTime time;
            string data = PlayerPrefs.GetString(key, "");
            if (String.IsNullOrEmpty(data))
                return defaultValue;

            if (data.TryDeserializeDateTime(out time))
                return time;

            return defaultValue;
        }

        private static void SetDateTimeValue(string key, DateTime value)
        {
            PlayerPrefs.SetString(key, value.Serialize());
        }
    }
}