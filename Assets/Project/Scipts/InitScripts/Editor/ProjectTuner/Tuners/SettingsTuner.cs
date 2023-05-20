using UnityEditor;
using UnityEngine;

namespace Project.Editor.Tuner
{
    public abstract class SettingsTuner
    {
        protected abstract string ApplyButtonName
        {
            get;
        }
        
        public virtual void DrawSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button($"{ApplyButtonName}"))
            {
                ApplySettings();
                
                Debug.Log("Save project...");
            }
        }
        
        public abstract void Init();
        protected abstract void ApplySettings();
    }
}