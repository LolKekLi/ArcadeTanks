using System.Linq;

namespace Project.Editor.Helpers
{
    public static class DefineHelper
    {
        public static string[] GetPlatformDefines()
        {
            UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildHelper.GetBuildTargetGroup(),
                out var defines);

            return defines;
        }

        public static bool IsDefineEnabled(string define, string[] defines = null)
        {
            if (defines == null)
            {
                defines = GetPlatformDefines();
            }
            
            return FindDefine(defines, define);
        }

        private static bool FindDefine(string[] defines, string define)
        {
            if (defines != null && defines.Length > 0)
            {
                if (defines.Contains(define))
                {
                    return true;
                }
            }

            return false;
        }
    }
}