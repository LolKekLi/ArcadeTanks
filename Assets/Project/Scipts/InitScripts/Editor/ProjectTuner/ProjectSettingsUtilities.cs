using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Project.Editor.Tuner.Custom;
using Project.Editor.Tuner.Init;
using Project.Editor.Tuner.Quality;
using Project.Editor.Tuner.Release;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Tuner
{
    public class ProjectSettingsUtilities : EditorWindow
    {
        private static readonly string ProjectSettingsWindowName = "Project Tuner";

        private static readonly Dictionary<int, ProjectTunerTabs> ProjectTunerTabPairs =
            new Dictionary<int, ProjectTunerTabs>();

        private static readonly Dictionary<ProjectTunerTabs, SettingsTuner> SettingsTuners =
            new Dictionary<ProjectTunerTabs, SettingsTuner>();
        
        private int _selectedTab = 0;
        
        [MenuItem("Tools/Project Tuner")]
        private static void ShowProjectSettingsTuner()
        {
            Init();
            
            ShowProjectTunerWindow();
        }

        private static void Init()
        {
            SettingsTuners.Clear();
            ProjectTunerTabPairs.Clear();

            var tabTypes = (ProjectTunerTabs[])Enum.GetValues(typeof(ProjectTunerTabs));

            foreach (var tabType in tabTypes)
            {
                ProjectTunerTabPairs.Add((int)tabType, tabType);
            }
            
            var tabs = ProjectTunerTabPairs.Values.ToArray();

            foreach (var tab in tabs)
            {
                SettingsTuners.Add(tab, SetupSettingsTuner(tab));
            }
        }

        private static SettingsTuner SetupSettingsTuner(ProjectTunerTabs tab)
        {
            SettingsTuner settingsTuner = null;
            
            switch (tab)
            {
                case ProjectTunerTabs.InitProjectTuner:
                    settingsTuner = new InitProjectTuner();
                    break;
                
                case ProjectTunerTabs.CustomProjectTuner:
                    settingsTuner = new ProjectSettingsTuner();
                    break;
                
                case ProjectTunerTabs.QualitySettingsTuner:
                    settingsTuner = new QualitySettingsTuner();
                    break;
                
                case ProjectTunerTabs.ReleaseSettingsTuner:
                    settingsTuner = new ReleaseSettingsTunner();
                    break;
                
                default:
                    Debug.LogError($"Not found {nameof(ProjectTunerTabs)} SettingsTuner for tab: {tab}");
                    break;
            }
            
            settingsTuner?.Init();
            
            return settingsTuner;
        }

        private static void ShowProjectTunerWindow()
        {
            ProjectSettingsUtilities window =
                (ProjectSettingsUtilities)GetWindow(typeof(ProjectSettingsUtilities));
            window.titleContent.text = ProjectSettingsWindowName;
            window.Show();
        }

        private void OnGUI()
        {
            var values = ProjectTunerTabPairs.Values.Select(t => SplitCamelCase(t.ToString())).ToArray();
            
            EditorGUILayout.BeginHorizontal();
            _selectedTab = GUILayout.Toolbar(_selectedTab, values);
            EditorGUILayout.EndHorizontal();

            if (ProjectTunerTabPairs.TryGetValue(_selectedTab, out var tab))
            {
                if (SettingsTuners.TryGetValue(tab, out var tuner))
                {
                    tuner.DrawSettings();
                }
                else
                {
                    Init();
                }
            }
        }
        
        private string SplitCamelCase(string source) 
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
        }
    }
}