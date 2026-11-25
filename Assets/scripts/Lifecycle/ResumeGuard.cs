using System;
using System.Threading;
using UnityEngine;

// Week 11 reference: a one-shot timer that survives pause/resume and never fires twice.
public class ResumeGuard : MonoBehaviour
{
    private CancellationTokenSource _cts;
    private bool _paused, _done;

    private void OnEnable() => LifecycleGuard.PausedChanged += OnPausedChanged;
    private void OnDisable() => LifecycleGuard.PausedChanged -= OnPausedChanged;
    private void Start() { RenewCts(); if (!_paused) _ = BossTimerAsync(_cts.Token); }
    private void OnDestroy()
    {
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); _cts = null; }
    }

    private void OnPausedChanged(bool paused)
    {
        if (paused == _paused) return;   // repeat (focus + pause), or launch (false)
        _paused = paused;
        RenewCts();                      // stop awaits
        if (!paused && !_done) _ = BossTimerAsync(_cts.Token); // only if not fired
    }

    private void RenewCts()
    {
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); }
        _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
    }

    private async Awaitable BossTimerAsync(CancellationToken ct)
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(5f, ct);
            if (_done || ct.IsCancellationRequested) return;
            Spawn();
        }
        catch (OperationCanceledException) { }   // on pause or exit
        catch (Exception e) { Debug.LogException(e); }
    }

    private void Spawn() { _done = true; Debug.Log("Boss spawned"); }
}
