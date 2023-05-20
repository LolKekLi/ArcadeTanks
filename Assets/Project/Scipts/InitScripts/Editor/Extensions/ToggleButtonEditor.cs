using Project.UI;
using UnityEditor;

namespace Project.Editor
{
    [CustomEditor(typeof(ToggleButton))]
    public class ToggleButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            SerializedObject so = new SerializedObject(target);
            
            SerializedProperty stringsProperty = so.FindProperty("_image");
            EditorGUILayout.PropertyField(stringsProperty, true);
            
            stringsProperty = so.FindProperty("_text");
            EditorGUILayout.PropertyField(stringsProperty, true);
            
            stringsProperty = so.FindProperty("_activeSprite");
            EditorGUILayout.PropertyField(stringsProperty, true);
            
            stringsProperty = so.FindProperty("_disabledSprite");
            EditorGUILayout.PropertyField(stringsProperty, true);
            
            so.ApplyModifiedProperties(); 
        }
    }
}