using System;
using System.Threading;
using UnityEngine;

public class FakePurchaseProvider : IPurchaseProvider
{
    const float Latency = 0.5f; // pretend store round-trip

    // ResumeGuard state for the latency timer
    CancellationTokenSource _cts;
    AwaitableCompletionSource<bool> _result;
    bool _paused, _done;
    float _start; // scaled Time.time when the purchase began

    public async Awaitable<bool> PurchaseAsync(string productId)
    {
        if (_result != null) return false;         // one purchase at a time
        Telemetry.Log("purchase_stub", ("product_id", productId));
        DevLog.Log($"[iap-stub] purchase {productId}");

        _result = new AwaitableCompletionSource<bool>();
        _paused = _done = false;
        _start = Time.time;
        LifecycleGuard.PausedChanged += OnPausedChanged;
        try
        {
            RenewCts();
            _ = WaitAsync(_cts.Token);
            return await _result.Awaitable;        // completes exactly once
        }
        finally
        {
            LifecycleGuard.PausedChanged -= OnPausedChanged;
            _cts.Cancel(); _cts.Dispose(); _cts = null;
            _result = null;
        }
    }

    void OnPausedChanged(bool paused)
    {
        if (paused == _paused || _cts == null) return;
        _paused = paused;
        RenewCts();
        if (!paused && !_done) _ = WaitAsync(_cts.Token);
    }

    void RenewCts()
    {
        if (_cts != null) { _cts.Cancel(); _cts.Dispose(); }
        _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
    }

    async Awaitable WaitAsync(CancellationToken ct)
    {
        try
        {
            float remaining = Latency - (Time.time - _start);
            if (remaining > 0f) await Awaitable.WaitForSecondsAsync(remaining, ct);
            if (_done || _paused || ct.IsCancellationRequested) return;
            _done = true;
            _result?.TrySetResult(true);
        }
        catch (OperationCanceledException)
        {
            // pause: OnPausedChanged restarts us. Exit: fail the purchase so the caller unwinds.
            if (Application.exitCancellationToken.IsCancellationRequested) _result?.TrySetResult(false);
        }
        catch (Exception e) { Debug.LogException(e); _result?.TrySetResult(false); }
    }
}
