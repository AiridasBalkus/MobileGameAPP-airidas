using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BuildStamp : MonoBehaviour
{
    public TMP_Text label;

    void OnEnable()
    {
        string guid = Application.buildGUID;
        string id = guid.Length >= 8 ? guid.Substring(0, 8) : guid;
        double hz = Screen.currentResolution.refreshRateRatio.value;
        label.text =
            $"v{Application.version} ({id})\n"
            + $"{SystemInfo.deviceModel}\n"
            + $"{SystemInfo.operatingSystem}\n"
            + $"{SystemInfo.systemMemorySize} MB RAM  "
            + $"{Screen.width}x{Screen.height} @ {hz:0} Hz"
            + "Feedback: https://forms.cloud.microsoft/e/YYyjNvHK6G"; ;
        Debug.Log($"build={Application.version}/{id}");
    }
    public void OpenFeedbackForm() =>
    Application.OpenURL("https://forms.cloud.microsoft/e/YYyjNvHK6G");
    void Update()
    {
        // Android back button arrives as Escape in the Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            gameObject.SetActive(false);
    }
}