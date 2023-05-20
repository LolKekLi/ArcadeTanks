using System.Collections.Generic;
using System.Linq;
using Project.Editor.Tuner.Init.Settings;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Project.Editor.Tuner.Init
{
    public class InitProjectTuner : SettingsTuner
    {
        private bool _isDeveloperSettingsEnabled;

        private int _selectedBuildTarget = 0;

        protected override string ApplyButtonName 
        { 
            get => "Init Project"; 
        }

        public override void Init() { }

        public override void DrawSettings()
        {
            _isDeveloperSettingsEnabled =
                EditorGUILayout.Toggle("Developer Settings Enabled: ", _isDeveloperSettingsEnabled);

            if (_isDeveloperSettingsEnabled)
            {
                _selectedBuildTarget = EditorGUILayout.Popup("Target Store", _selectedBuildTarget,
                    InitProjectSettings.BuildTargetGroups.Values.Select(s => s.ToString()).ToArray());
            }
            else
            {
                _selectedBuildTarget = 0;
            }
            
            base.DrawSettings();
        }

        protected override void ApplySettings()
        {
            SwitchBuildTargetPlatform();
            SetupPackageManager();
            SetupSpritePackerMode();
            SetupScriptingBackend();
            SetupDebugMenu();
            SetupEditorBuildSettingsScene();
        }

        private void SwitchBuildTargetPlatform()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                    InitProjectSettings.BuildTargetGroups[_selectedBuildTarget],
                    InitProjectSettings.BuildTargets[_selectedBuildTarget]);
            }
        }

        private void SetupScriptingBackend()
        {
            UnityEditor.PlayerSettings.SetScriptingBackend(InitProjectSettings.BuildTargetGroups[_selectedBuildTarget],
                ScriptingImplementation.IL2CPP);
        }

        private void SetupPackageManager()
        {
            foreach (var a in InitProjectSettings.PackagePaths)
            {
                Client.Add(a);
            }
        }

        private void SetupSpritePackerMode()
        {
            EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
        }

        private void SetupEditorBuildSettingsScene()
        {
            var scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
            var scenesPaths = scenesGUIDs.Select(AssetDatabase.GUIDToAssetPath);

            List<string> foundedScenes = new List<string>();

            List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
            foreach (var initialScene in InitProjectSettings.InitialScenes)
            {
                foreach (var scene in scenesPaths)
                {
                    if (scene.Contains(initialScene))
                    {
                        foundedScenes.Add(scene);
                    }
                }
            }

            for (int i = 0; i < foundedScenes.Count; i++)
            {
                if (buildScenes.Select(bs => bs.path).All(bs => !bs.Contains(foundedScenes[i])))
                {
                    buildScenes.Insert(i, new EditorBuildSettingsScene(foundedScenes[i], true));
                }
                else
                {
                    int index = -1;

                    for (int j = 0; j < buildScenes.Count; j++)
                    {
                        if (buildScenes[j].path.Equals(foundedScenes[i]))
                        {
                            index = j;
                            
                            break;
                        }
                    }

                    var editorScene = buildScenes[index];
                    
                    buildScenes.Remove(editorScene);
                    buildScenes.Insert(i, editorScene);
                }
            }
            
            foreach (var scenePath in foundedScenes)
            {
                if (buildScenes.Select(bs => bs.path).All(bs => !bs.Contains(scenePath)))
                {
                    buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }
            
            EditorBuildSettings.scenes = buildScenes.ToArray();
        }

        private void SetupDebugMenu()
        {
            var settings = Resources.Load<SRDebugger.Settings>("SRDebugger/" + InitProjectSettings.DebugResourcesName);
            settings.IsEnabled = false;
        }
    }
}