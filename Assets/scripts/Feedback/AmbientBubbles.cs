using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Ambient bubbles in the Game scene, skipped on Low tier
public static class AmbientBubbles
{
    const string GameScene = "Game";
    const string BubbleTex = "MinoUW/FX/FX_Bubble";
    const string BubbleTexOld = "MinoUW/UI/T_Bubble";
    const string UrpParticles = "Universal Render Pipeline/Particles/Unlit";
    static Material s_mat;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Hook()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        var active = SceneManager.GetActiveScene();
        if (active.name == GameScene) Create();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameScene) Create();
    }

    static void Create()
    {
        if (DeviceTier.IsLow) return;
        var cam = Camera.main;
        if (!cam || cam.transform.Find("AmbientBubbles")) return;
        var mat = GetMaterial();
        if (!mat) return;

        var go = new GameObject("AmbientBubbles");
        go.transform.SetParent(cam.transform, false);
        go.transform.localPosition = new Vector3(0f, 0f, 16f);

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // must stop before setting duration

        var main = ps.main;
        main.duration = 5f;
        main.loop = true;
        main.startLifetime = 4f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.35f);
        main.startColor = new Color(1f, 1f, 1f, 0.7f);
        main.gravityModifier = 0f;
        main.maxParticles = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 12f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(14f, 8f, 26f);

        var vel = ps.velocityOverLifetime; // all axes need the same curve mode
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.World;
        vel.x = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);
        vel.y = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var r = go.GetComponent<ParticleSystemRenderer>();
        r.renderMode = ParticleSystemRenderMode.Billboard;
        r.sharedMaterial = mat;
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;

        ps.Play();
    }

    static Material GetMaterial()
    {
        if (s_mat) return s_mat;
        var tex = Resources.Load<Texture2D>(BubbleTex);
        if (!tex) tex = Resources.Load<Texture2D>(BubbleTexOld);
        if (!tex) { Debug.LogWarning("[Bubbles] missing Resources/" + BubbleTex); return null; }

        var shader = Shader.Find(UrpParticles);
        if (shader)
        {
            // transparent, alpha blend
            s_mat = new Material(shader) { name = "M_Bubble_Runtime" };
            s_mat.SetTexture("_BaseMap", tex);
            s_mat.SetFloat("_Surface", 1f);
            s_mat.SetFloat("_Blend", 0f);
            s_mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            s_mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            s_mat.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            s_mat.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
            s_mat.SetFloat("_ZWrite", 0f);
            s_mat.SetOverrideTag("RenderType", "Transparent");
            s_mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            s_mat.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            // shader not in build
            shader = Shader.Find("Sprites/Default");
            if (!shader) { Debug.LogWarning("[Bubbles] no particle shader in build"); return null; }
            Debug.LogWarning("[Bubbles] " + UrpParticles + " not in build, using Sprites/Default. Add it to Always Included Shaders.");
            s_mat = new Material(shader) { name = "M_Bubble_Runtime", mainTexture = tex };
        }
        return s_mat;
    }
}
