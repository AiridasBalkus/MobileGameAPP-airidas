using System.Diagnostics;

// Logs only in the Editor and Development builds; calls are stripped from release builds
public static class DevLog
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message) => UnityEngine.Debug.Log(message);
}
