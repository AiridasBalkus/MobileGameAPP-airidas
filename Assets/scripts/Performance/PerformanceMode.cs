using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PerfMode { Balanced, HighFps }

public class PerformanceMode : MonoBehaviour
{
    const string Key = "perf_mode";
    const float HighFpsScale = 0.8f;
    public static PerformanceMode Instance { get; private set; }
    public static bool IsHighFps => PlayerPrefs.GetInt(Key, 0) == 1;
    float _assetScale = 1f;
    float _tierScale = 1f;

    void Awake() { Instance = this; DeviceTier.Apply(); }
    void Start()
    {
        if (UniversalRenderPipeline.asset) _assetScale = UniversalRenderPipeline.asset.renderScale;
        // full res until the tier URP assets exist
        _tierScale = DeviceTier.QualityApplied ? _assetScale : 1f;
        Apply(IsHighFps ? PerfMode.HighFps : PerfMode.Balanced);
    }
    public void Apply(PerfMode mode)
    {
        bool high = mode == PerfMode.HighFps;
        Application.targetFrameRate = high ? 120 : (DeviceTier.IsLow ? 30 : 60);
        var urp = UniversalRenderPipeline.asset;
        if (urp) urp.renderScale = high ? Mathf.Min(_tierScale, HighFpsScale) : _tierScale;
        PlayerPrefs.SetInt(Key, high ? 1 : 0); PlayerPrefs.Save();
        Debug.Log($"[perf] perf_mode={mode} high={high} tier={DeviceTier.CurrentName} fps={Application.targetFrameRate} " +
                  $"scale={(urp ? urp.renderScale : -1f):0.00} asset={(urp ? urp.name : "none")}");
    }
    public void SetHighFps(bool on) => Apply(on ? PerfMode.HighFps : PerfMode.Balanced);

#if UNITY_EDITOR
    void OnDestroy() { if (UniversalRenderPipeline.asset) UniversalRenderPipeline.asset.renderScale = _assetScale; }
#endif
}
