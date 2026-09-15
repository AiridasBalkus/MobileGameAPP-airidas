using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"[Boot] {Application.productName} v{Application.version} " +
                  $"| Platform: {Application.platform} " +
                  $"| Device: {SystemInfo.deviceModel} " +
                  $"| Screen: {Screen.width}x{Screen.height} @ {Screen.dpi}dpi");
    }
}
