using System;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project
{
    public static class DebugSafe
    {
        [Conditional("FORCE_DEBUG")]
        public static void Log(object message) => UnityEngine.Debug.unityLogger.Log(LogType.Log, message);
        
        [Conditional("FORCE_DEBUG")]
        public static void LogError(object message) => UnityEngine.Debug.unityLogger.Log(LogType.Error, message);
        
        [Conditional("FORCE_DEBUG")]
        public static void LogException(Exception exception) => UnityEngine.Debug.unityLogger.LogException(exception, (Object) null);
        
        [Conditional("FORCE_DEBUG")]
        public static void LogWarning(object message) => UnityEngine.Debug.unityLogger.Log(LogType.Warning, message);
    }
}