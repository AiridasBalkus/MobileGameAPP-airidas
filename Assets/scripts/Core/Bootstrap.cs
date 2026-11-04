using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        string guid = Application.buildGUID;
        string build = guid.Length >= 8 ? guid.Substring(0, 8) : guid;
        Telemetry.Log("session_start",
            ("version", Application.version),
            ("build", build),
            ("device", SystemInfo.deviceModel),
            ("android", SystemInfo.operatingSystem));

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
