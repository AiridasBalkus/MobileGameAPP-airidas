using System;
using System.Threading;
using UnityEngine;

public class StoreShot : MonoBehaviour
{
    [SerializeField] private GameObject hudRoot;   // HUD to hide
    [SerializeField] private int superSize = 1;    // 2 = double res
    const float WriteTime = 0.5f;                  // file write

    // ResumeGuard state for the HUD-restore timer
    CancellationTokenSource _cts;
    bool _paused;       // last value seen from LifecycleGuard
    bool _busy;         // capture in progress (HUD hidden)
    bool _restoreOwed;  // shot taken, HUD not restored yet (the one-shot's "not done")
    float _shotTime;    // scaled Time.time of the capture
    string _file;

    void OnEnable() => LifecycleGuard.PausedChanged += OnPausedChanged;
    void OnDisable()
    {
        LifecycleGuard.PausedChanged -= OnPausedChanged;
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); _cts = null; }
        if (_busy && hudRoot) hudRoot.SetActive(true);
        _busy = _restoreOwed = false;
    }

    public async Awaitable CaptureAsync(string tag)
    {
        if (_busy) return;                         // one capture at a time
        _busy = true;
        hudRoot.SetActive(false);
        try
        {
            await Awaitable.NextFrameAsync(destroyCancellationToken); // HUD is gone now
            _file = $"{tag}_{Screen.width}x{Screen.height}.png";
            // Android: Unity prepends Application.persistentDataPath
            ScreenCapture.CaptureScreenshot(_file, superSize);
        }
        catch (OperationCanceledException) { return; }
        catch (Exception e) { Debug.LogException(e); hudRoot.SetActive(true); _busy = false; return; }

        _shotTime = Time.time;
        _restoreOwed = true;
        RenewCts();
        if (!_paused) await RestoreHudAsync(_cts.Token); // else OnPausedChanged restarts it
    }

    void OnPausedChanged(bool paused)
    {
        if (paused == _paused) return;
        _paused = paused;
        if (_cts == null) return;                  // no timer started yet
        RenewCts();
        if (!paused && _restoreOwed) _ = RestoreHudAsync(_cts.Token);
    }

    void RenewCts()
    {
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); }
        _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
    }

    async Awaitable RestoreHudAsync(CancellationToken ct)
    {
        try
        {
            float remaining = WriteTime - (Time.time - _shotTime);
            if (remaining > 0f) await Awaitable.WaitForSecondsAsync(remaining, ct);
            if (!_restoreOwed || _paused || ct.IsCancellationRequested) return;
            _restoreOwed = false;
            _busy = false;
            hudRoot.SetActive(true);
            Debug.Log($"Saved {Application.persistentDataPath}/{_file}");
        }
        catch (OperationCanceledException) { }     // pause or exit
        catch (Exception e) { Debug.LogException(e); }
    }
}
