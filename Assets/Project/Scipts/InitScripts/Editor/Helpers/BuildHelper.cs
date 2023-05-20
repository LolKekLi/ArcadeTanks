using UnityEditor;

namespace Project.Editor.Helpers
{
    public static class BuildHelper
    {
        public static BuildTargetGroup GetBuildTargetGroup()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);

            return group;
        }
    }
}