using UnityEditor;

namespace Project.Editor.Tuner.Release.Settings
{
    public class ReleaseSettings
    {
        public AndroidArchitecture Architecture
        {
            get;
            private set;
        }

        public ReleaseSettings(AndroidArchitecture architecture)
        {
            Architecture = architecture;
        }
    }
}