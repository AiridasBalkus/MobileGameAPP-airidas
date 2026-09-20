using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class HapticsToggle : MonoBehaviour
{
    Toggle _toggle;
    void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    void OnEnable()
    {
        // Reflect the saved preference without firing OnValueChanged.
        _toggle.SetIsOnWithoutNotify(Haptics.Enabled);
        _toggle.onValueChanged.AddListener(OnToggled);
    }

    void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnToggled);
    }

    void OnToggled(bool value)
    {
        Haptics.Enabled = value;
    }
}
