using UnityEngine;
using System.Threading;
using System;

[RequireComponent(typeof(AudioSource))]
public class Feedback : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float shakeStrength = 0.3f;
    AudioSource _audio;
    AudioClip _click;
    Vector3 _camHome;
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _click = MakeClick();                     // built once at start, no asset needed
        _camHome = cameraTransform.localPosition;
    }

    public void LaneChange() { _audio.pitch = 1f; _audio.PlayOneShot(_click, 0.4f); }
    public void Pickup() { _audio.pitch = 1.6f; _audio.PlayOneShot(_click, 0.6f); }
    public void Hit() => _ = ShakeAsync(destroyCancellationToken);

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
        finally { if (cameraTransform) cameraTransform.localPosition = _camHome; }
    }
    //this is a placeholder click sound until CA3 when ill replace it with a proper sound effect
    static AudioClip MakeClick()
    {
        const int rate = 44100;
        int n = rate / 25;
        var data = new float[n];
        for (int i = 0; i < n; i++)
            data[i] = Mathf.Sin(2f * Mathf.PI * 1200f * i / rate) * (1f - (float)i / n);
        var clip = AudioClip.Create("click", n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
