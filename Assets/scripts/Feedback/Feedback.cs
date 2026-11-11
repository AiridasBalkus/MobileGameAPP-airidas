using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;

public class Feedback : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeStrength = 0.3f;
    const float FlashTime = 0.35f;
    const float FlashAlpha = 0.45f;
    Vector3 _camHome;
    Image _flash;
    void Awake()
    {
        _camHome = cameraTransform.localPosition;
    }

    void Start() => _flash = CreateFlash();

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
            float total = Mathf.Max(shakeDuration, FlashTime);
            if (_flash) _flash.enabled = true;
            for (float t = 0f; t < total; t += Time.deltaTime)
            {
                float fade = Mathf.Clamp01(1f - t / shakeDuration);
                cameraTransform.localPosition = _camHome + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeStrength * fade);
                if (_flash) _flash.color = new Color(1f, 0.15f, 0.1f, FlashAlpha * (1f - t / FlashTime));
                await Awaitable.NextFrameAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
        finally
        {
            if (cameraTransform) cameraTransform.localPosition = _camHome;
            if (_flash) _flash.enabled = false;
        }
    }

    // full-screen red flash on hit, over the HUD
    static Image CreateFlash()
    {
        var canvas = GameObject.Find("Canvas");
        if (!canvas) return null;
        var go = new GameObject("HitFlash", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(canvas.transform, false);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.raycastTarget = false;
        img.enabled = false;
        return img;
    }
}
