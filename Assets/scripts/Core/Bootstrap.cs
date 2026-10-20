using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        Telemetry.Log("session_start",
        ("app_version", Application.version),
        ("device_model", SystemInfo.deviceModel),
        ("android", SystemInfo.operatingSystem));
        Debug.Log($"[telemetry] file {Telemetry.FilePath}");

        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log($"[Boot] {SystemInfo.deviceModel} | " +
                  $"{SystemInfo.operatingSystem} | " +
                  $"{SystemInfo.graphicsDeviceType} | " +
                  $"{Screen.width}x{Screen.height} @ {Screen.dpi} dpi");
        DontDestroyOnLoad(gameObject);
    }
    async Awaitable Start() => await SceneLoader.Instance.Load("Menu", Application.exitCancellationToken);
    public void Load(string sceneName)
    {
        if (SceneLoader.Instance) _ = SceneLoader.Instance.Load(sceneName, Application.exitCancellationToken);
        else SceneManager.LoadScene(sceneName);
    }
}
