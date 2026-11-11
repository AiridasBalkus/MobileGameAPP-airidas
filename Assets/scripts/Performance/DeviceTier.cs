using UnityEngine;

public class DeviceTier : MonoBehaviour
{
    public const int Low = 0, Mid = 1, High = 2; // Quality level order
    public const string OverrideKey = "tier_override";
    static readonly string[] Names = { "Low", "Mid", "High" };

    // phones report less than their nominal RAM: S24 Ultra 12 GB = 11084 MB,
    // S25 FE 8 GB = 7204 MB, A15 4 GB = 3638 MB, so the lab 8192/4096 thresholds are too high
    const int HighRamMb = 7000, MidRamMb = 3500;
    const int HighCores = 8, MidCores = 6;

    public static int Current { get; private set; } = Mid;
    public static string CurrentName => Names[Current];
    public static bool IsLow => Current == Low;
    public static bool IsOverridden { get; private set; }
    public static bool QualityApplied { get; private set; }
    static bool s_applied;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() => Apply();

    void Awake() => Apply();

    public static void Apply()
    {
        if (s_applied) return;
        s_applied = true;

        int stored = PlayerPrefs.GetInt(OverrideKey, -1);
        IsOverridden = stored >= Low && stored <= High;
        Current = IsOverridden ? stored : Detect();

        // only if the level exists
        int level = System.Array.IndexOf(QualitySettings.names, CurrentName);
        QualityApplied = level >= 0;
        if (QualityApplied) QualitySettings.SetQualityLevel(level, true);
        else Debug.LogWarning($"[perf] quality level '{CurrentName}' not found, keeping '{QualitySettings.names[QualitySettings.GetQualityLevel()]}'");

        Application.targetFrameRate = Current == Low ? 30 : 60;
        Debug.Log($"[perf] tier={CurrentName} override={IsOverridden} ram={SystemInfo.systemMemorySize}MB cores={SystemInfo.processorCount} quality={QualitySettings.names[QualitySettings.GetQualityLevel()]}");
    }

    static int Detect()
    {
        int ram = SystemInfo.systemMemorySize;   // MB
        int cores = SystemInfo.processorCount;
        if (ram >= HighRamMb && cores >= HighCores) return High;
        if (ram >= MidRamMb && cores >= MidCores) return Mid;
        return Low;
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { s_applied = false; Current = Mid; IsOverridden = false; QualityApplied = false; }
#endif
}
