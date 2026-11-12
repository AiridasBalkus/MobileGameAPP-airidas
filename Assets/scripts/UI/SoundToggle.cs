using UnityEngine;
using UnityEngine.UI;

// Sound on/off
[RequireComponent(typeof(Toggle))]
public class SoundToggle : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Sprite onIcon;
    [SerializeField] Sprite offIcon;
    Toggle _toggle;

    void Awake() => _toggle = GetComponent<Toggle>();

    void OnEnable()
    {
        _toggle.SetIsOnWithoutNotify(AudioService.SoundOn);
        _toggle.onValueChanged.AddListener(OnToggled);
        ShowIcon(AudioService.SoundOn);
    }

    void OnDisable() => _toggle.onValueChanged.RemoveListener(OnToggled);

    void OnToggled(bool on)
    {
        AudioService.SoundOn = on;
        ShowIcon(on);
    }

    void ShowIcon(bool on)
    {
        var sprite = on ? onIcon : offIcon;
        if (icon && sprite) icon.sprite = sprite;
    }
}
