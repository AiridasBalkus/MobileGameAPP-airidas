using UnityEngine;
using UnityEngine.UI;

// Sound on/off
[RequireComponent(typeof(Toggle))]
public class SoundToggle : MonoBehaviour
{
    Toggle _toggle;
    void Awake() => _toggle = GetComponent<Toggle>();

    void OnEnable()
    {
        _toggle.SetIsOnWithoutNotify(AudioService.SoundOn);
        _toggle.onValueChanged.AddListener(OnToggled);
    }

    void OnDisable() => _toggle.onValueChanged.RemoveListener(OnToggled);

    void OnToggled(bool on) => AudioService.SoundOn = on;
}
