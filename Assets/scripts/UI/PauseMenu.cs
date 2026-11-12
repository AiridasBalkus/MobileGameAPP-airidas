using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] LifecycleGuard guard;
    // button only opened the panel, never paused (KI-03)
    [SerializeField] Button pauseButton;
    GameManager _game;

    void Awake()
    {
        if (!guard) guard = FindAnyObjectByType<LifecycleGuard>();
        _game = FindAnyObjectByType<GameManager>();
        if (!pauseButton)
        {
            var t = transform.Find("SafeArea/Pause");
            if (t) pauseButton = t.GetComponent<Button>();
        }
        if (pauseButton) pauseButton.onClick.AddListener(OnPausePressed);
        else Debug.LogWarning("[Pause] HUD Pause button not found (expected Canvas/SafeArea/Pause)");
    }

    void OnDestroy() { if (pauseButton) pauseButton.onClick.RemoveListener(OnPausePressed); }

    void OnEnable() { LifecycleGuard.PausedChanged += Show; Show(LifecycleGuard.IsPaused); }
    void OnDisable() { LifecycleGuard.PausedChanged -= Show; }
    void Show(bool paused) => panel.SetActive(paused);

    void Update()
    {
        if (SceneLoader.IsLoading) return;
        var kb = Keyboard.current;
        if (kb == null || !kb.escapeKey.wasPressedThisFrame) return;
        if (LifecycleGuard.IsPaused) guard.SetPaused(false);
        else OnPausePressed();
    }

    public void OnPausePressed()
    {
        // don't pause after the run has ended
        bool ended = _game && (_game.Current == GameManager.State.Won || _game.Current == GameManager.State.Lost);
        if (ended || !guard) { panel.SetActive(false); return; }
        if (!LifecycleGuard.IsPaused) guard.SetPaused(true);
    }

    public void OnResumePressed() => guard.SetPaused(false);
}
