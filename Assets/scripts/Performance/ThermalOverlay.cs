using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
public class ThermalOverlay : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    static readonly string[] Formats =
    {
        "{0:1} ms  NoWarning\ntemp {1:2}  trend {2:2}",
        "{0:1} ms  ThrottlingImminent\ntemp {1:2}  trend {2:2}",
        "{0:1} ms  Throttling\ntemp {1:2}  trend {2:2}",
    };
    IAdaptivePerformance ap;
    float _sum; int _frames; float _next;

    void Start()
    {
        ap = Holder.Instance;
        if (ap != null && ap.Active) ap.ThermalStatus.ThermalEvent += OnThermal;
        else Debug.Log("[AP] inactive");
    }

    void OnDestroy() { if (ap != null && ap.Active) ap.ThermalStatus.ThermalEvent -= OnThermal; }

    void Update()
    {
        _sum += Time.unscaledDeltaTime; _frames++;
        if (Time.unscaledTime < _next) return;
        _next = Time.unscaledTime + 0.5f;
        float ms = _sum / _frames * 1000f; _sum = 0f; _frames = 0;

        if (ap == null || !ap.Active) { label.SetText("{0:1} ms  (AP inactive)", ms); return; }
        var t = ap.ThermalStatus.ThermalMetrics;
        label.SetText(Formats[(int)t.WarningLevel], ms, t.TemperatureLevel, t.TemperatureTrend);
    }
    void OnThermal(ThermalMetrics ev) =>
        Debug.Log($"[AP] thermal warning={ev.WarningLevel} level={ev.TemperatureLevel:F2} " +
                  $"trend={ev.TemperatureTrend:F2} t={Time.realtimeSinceStartup:F0}s");
}