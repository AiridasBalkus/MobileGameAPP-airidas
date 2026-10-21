using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PerfModeToggle : MonoBehaviour
{
    Toggle _toggle;
    void Awake() => _toggle = GetComponent<Toggle>();
    void OnEnable()
    {
        _toggle.SetIsOnWithoutNotify(PerformanceMode.IsHighFps);
        _toggle.onValueChanged.AddListener(OnToggled);
    }
    void OnDisable() => _toggle.onValueChanged.RemoveListener(OnToggled);
    void OnToggled(bool on) { if (PerformanceMode.Instance) PerformanceMode.Instance.SetHighFps(on); }
}