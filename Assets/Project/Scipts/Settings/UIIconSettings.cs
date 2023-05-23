using System;
using System.Linq;
using Project;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/UIIconSettings", fileName = "UIIconSettings", order = 0)]
public class UIIconSettings : ScriptableObject
{
    public class UIIconPreset<T>
    {
        [field: SerializeField]
        public T Type
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Sprite Icon
        {
            get;
            private set;
        }
    }

    [Serializable]
    public class TurretIconPreset : UIIconPreset<TurretType>
    {
    }
    
    [Serializable]
    public class BodyIconPreset : UIIconPreset<BodyType>
    {
        
    }

    [field: SerializeField]
    public TurretIconPreset[] TurretIconPresets
    {
        get;
        private set;
    }

    [field: SerializeField]
    public BodyIconPreset[] BodyIconPresets
    {
        get;
        private set;
    }

    public Sprite GetBodyIcon(BodyType type)
    {
        var bodyIconPreset = BodyIconPresets.FirstOrDefault(x=>x.Type == type);

        if (bodyIconPreset == null)
        {
            Debug.Log($"Не найден пресет для типа {type}");
            return null;
        }

        return bodyIconPreset.Icon;
    }

    public Sprite GetTurretIcon(TurretType type)
    {
        var turretIconPreset = TurretIconPresets.FirstOrDefault(x=>x.Type == type);

        if (turretIconPreset == null)
        {
            Debug.Log($"Не найден пресет для типа {type}");
            return null;
        }

        return turretIconPreset.Icon;
    }
}