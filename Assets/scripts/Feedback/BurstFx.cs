using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// One-shot particle bursts (coin sparkle, hit sand puff), built once and fired with Emit
public class BurstFx
{
    readonly ParticleSystem _ps;
    readonly int _count;
    static readonly Dictionary<string, Material> s_mats = new Dictionary<string, Material>(); // one per texture per session

    BurstFx(ParticleSystem ps, int count) { _ps = ps; _count = count; }

    public static BurstFx Create(string texName, int count, float size, float speed, float lifetime, Color color, float gravity)
    {
        if (!s_mats.TryGetValue(texName, out var mat) || !mat)
        {
            var tex = Resources.Load<Texture2D>("MinoUW/FX/" + texName);
            if (!tex) { Debug.LogWarning("[FX] missing Resources/MinoUW/FX/" + texName); return null; }
            mat = AmbientBubbles.CreateParticleMaterial(tex);
            if (!mat) return null;
            s_mats[texName] = mat;
        }

        var go = new GameObject(texName);
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // must stop before setting duration

        var main = ps.main;
        main.duration = 1f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = lifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.5f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.5f, size);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = color;
        main.gravityModifier = gravity;
        main.maxParticles = count * 3;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var fade = ps.colorOverLifetime;
        fade.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                  new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        fade.color = g;

        var r = go.GetComponent<ParticleSystemRenderer>();
        r.renderMode = ParticleSystemRenderMode.Billboard;
        r.sharedMaterial = mat;
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;

        return new BurstFx(ps, count);
    }

    public void Play(Vector3 position)
    {
        if (!_ps) return;
        _ps.transform.position = position;
        _ps.Emit(_count);
    }
}
