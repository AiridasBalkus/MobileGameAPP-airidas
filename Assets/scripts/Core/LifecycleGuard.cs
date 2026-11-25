using UnityEngine;
public class LifecycleGuard : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    public static event System.Action<bool> PausedChanged;

    void Start() => SetPaused(false);
    // Home, app switch, incoming call, screen off: save HERE.
    void OnApplicationPause(bool paused)
    {
        // Resuming is a player choice (Resume button). Wallet, score and run state are
        // still in memory, so nothing is reloaded here.
        if (!paused) return;
        if (Wallet.Instance) WalletSave.Save(Wallet.Instance); // coins + level to disk
        PlayerPrefs.Save();            // flush everything else (highscore, hint runs, settings)
        SetPaused(true);
    }

    // Notification shade, permission dialog, on-screen keyboard.
    void OnApplicationFocus(bool f) { if (!f) SetPaused(true); }

    public void SetPaused(bool value)  // Resume button passes false
    {
        IsPaused = value;
        Time.timeScale = value ? 0f : 1f;
        AudioListener.pause = value;
        PausedChanged?.Invoke(value);
        Debug.Log($"[Pause] {value}, timeScale {Time.timeScale}");
    }
}