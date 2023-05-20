using System;
using Project.Editor.Helpers;
using Project.Editor.Tuner.Custom.Settings;
using Project.Editor.Tuner.Release;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Project
{
    public class BuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get => 0;
        }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            if (CompanyHelper.GetTargetCompany() == CompanyType.Vaveda)
            {
                if (DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.DebugMenuDefine))
                {
                    Debug.LogException(
                        new Exception($"Debug Menu Enabled! Go to Release Project Tunner Tab and click 'Setup'"));
                }

                if (DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.SayKitDefine))
                {
                    if (BuildHelper.GetBuildTargetGroup() == BuildTargetGroup.Android)
                    {
                        if (UnityEditor.PlayerSettings.Android.targetArchitectures !=
                            ReleaseSettingsTunner.SayKitSettings.Architecture)
                        {
                            Debug.LogException(new Exception(
                                $"Incorrect Target Architectures! Go to Release Project Tunner Tab and click 'Setup'"));
                        }
                    }
                }
                else
                {
                    if (BuildHelper.GetBuildTargetGroup() == BuildTargetGroup.Android)
                    {
                        if (UnityEditor.PlayerSettings.Android.targetArchitectures !=
                            ReleaseSettingsTunner.VavedaSettings.Architecture)
                        {
                            Debug.LogException(new Exception(
                                $"Incorrect Target Architectures! Go to Release Project Tunner Tab and click 'Setup'"));
                        }
                    }
                }
            }
        }
    }
}