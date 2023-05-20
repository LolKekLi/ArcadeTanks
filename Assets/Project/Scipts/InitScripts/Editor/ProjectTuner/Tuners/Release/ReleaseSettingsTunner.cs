using Project.Editor.Helpers;
using Project.Editor.Tuner.Custom.Settings;
using Project.Editor.Tuner.Release.Settings;
using UnityEditor;

namespace Project.Editor.Tuner.Release
{
    public class ReleaseSettingsTunner : SettingsTuner
    {
        public static readonly ReleaseSettings VavedaSettings = new ReleaseSettings(AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7);
        
        public static readonly ReleaseSettings SayKitSettings =
            new ReleaseSettings(AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7);
        
        protected override string ApplyButtonName
        {
            get => "Setup Release Settings";
        }
        
        public override void Init() { }

        protected override void ApplySettings()
        {
            ApplyReleaseSettings();
        }

        private void ApplyReleaseSettings()
        {
            if (DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.SayKitDefine))
            {
                UnityEditor.PlayerSettings.Android.targetArchitectures = SayKitSettings.Architecture;
            }
            else
            {
                UnityEditor.PlayerSettings.Android.targetArchitectures = VavedaSettings.Architecture;
            }
        }
    }
}
