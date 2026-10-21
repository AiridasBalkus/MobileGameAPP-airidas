using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PerfMode { Balanced, HighFps }

public class PerformanceMode : MonoBehaviour
{
    const string Key = "perf_mode";
    public static PerformanceMode Instance { get; private set; }
    public static bool IsHighFps => PlayerPrefs.GetInt(Key, 0) == 1;
    float _editorScale = 1f;

    void Awake() => Instance = this;
    void Start()
    {
        if (UniversalRenderPipeline.asset) _editorScale = UniversalRenderPipeline.asset.renderScale;
        Apply(IsHighFps ? PerfMode.HighFps : PerfMode.Balanced);
    }
    public void Apply(PerfMode mode)
    {
        bool high = mode == PerfMode.HighFps;
        Application.targetFrameRate = high ? 120 : 60;
        var urp = UniversalRenderPipeline.asset;
        if (urp) urp.renderScale = high ? 0.8f : 1.0f;
        PlayerPrefs.SetInt(Key, high ? 1 : 0); PlayerPrefs.Save();
        Debug.Log($"[perf] perf_mode={mode} high={high} fps={Application.targetFrameRate} " +
                  $"scale={(urp ? urp.renderScale : -1f):0.0} asset={(urp ? urp.name : "none")}");
    }
    public void SetHighFps(bool on) => Apply(on ? PerfMode.HighFps : PerfMode.Balanced);

#if UNITY_EDITOR
    void OnDestroy() { if (UniversalRenderPipeline.asset) UniversalRenderPipeline.asset.renderScale = _editorScale; }
#endif
}