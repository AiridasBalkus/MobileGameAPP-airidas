using UnityEngine;
using System.Threading;
using System;

public class Feedback : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeStrength = 0.3f;
    Vector3 _camHome;
    void Awake()
    {
        _camHome = cameraTransform.localPosition;
    }

    public void LaneChange() { var a = AudioService.Instance; if (a) a.Swipe(); }
    public void Pickup() { var a = AudioService.Instance; if (a) a.Coin(); }
    public void Hit()
    {
        var a = AudioService.Instance; if (a) a.Hit();
        _ = ShakeAsync(destroyCancellationToken);
    }

    async Awaitable ShakeAsync(CancellationToken ct)
    {
        try
        {
            for (float t = 0f; t < shakeDuration; t += Time.deltaTime)
            {
                float fade = 1f - t / shakeDuration;
                cameraTransform.localPosition = _camHome + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeStrength * fade);
                await Awaitable.NextFrameAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
        finally { if (cameraTransform) cameraTransform.localPosition = _camHome; }
    }
}
