using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class AdaptiveBootstrap : MonoBehaviour
{
    IAdaptivePerformance ap;

    void Start()
    {
        ap = Holder.Instance;
        if (ap == null || !ap.Active)
        {
            DevLog.Log("[AP] provider inactive on this device");
            return;
        }
        ap.ThermalStatus.ThermalEvent += OnThermal;
        ap.PerformanceStatus.PerformanceBottleneckChangeEvent += OnBottleneck;
        var m = ap.ThermalStatus.ThermalMetrics;
        DevLog.Log($"[AP] active warning={m.WarningLevel} level={m.TemperatureLevel:0.00}");
    }

    void OnDestroy()
    {
        if (ap == null) return;
        ap.ThermalStatus.ThermalEvent -= OnThermal;
        ap.PerformanceStatus.PerformanceBottleneckChangeEvent -= OnBottleneck;
    }

    void OnThermal(ThermalMetrics m)
    {
        DevLog.Log($"[AP] thermal warning={m.WarningLevel} " +
                  $"level={m.TemperatureLevel:0.00} trend={m.TemperatureTrend:0.00}");
    }

    void OnBottleneck(PerformanceBottleneckChangeEventArgs e)
    {
        DevLog.Log($"[AP] bottleneck={e.PerformanceBottleneck}");
    }
}