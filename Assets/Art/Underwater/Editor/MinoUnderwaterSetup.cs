// Mino underwater art pack v2: import settings (Week 9 Lab A budgets), materials, lighting and scenery.
// Everything under Assets/Art/Underwater/ gets its Android overrides automatically on import.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MinoUnderwaterImport : AssetPostprocessor
{
    const string Root = "Assets/Art/Underwater/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(Root)) return;
        var ti = (TextureImporter)assetImporter;
        string file = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        bool isUi = assetPath.Contains("/UI/") && file != "T_Bubble";
        bool isFx = assetPath.Contains("/FX/") || file == "T_Bubble";
        bool isCutout = file.StartsWith("S_");
        bool isSky = file.Contains("Panorama");
        int maxSize = 512;
        bool astc4 = false;

        if (isUi)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            if (file.StartsWith("UI_Button")) ti.spriteBorder = new Vector4(48, 48, 48, 48);
            if (file == "UI_Panel") ti.spriteBorder = new Vector4(60, 60, 60, 60);
            maxSize = (file.Contains("Logo") || file.Contains("AppIcon")) ? 1024 : file.StartsWith("Icon_") ? 128 : 512;
            astc4 = true;
        }
        else if (isFx)
        {
            ti.textureType = TextureImporterType.Default;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = false;
            ti.wrapMode = TextureWrapMode.Clamp;
            maxSize = 256;
            astc4 = true;
        }
        else if (isCutout)
        {
            ti.textureType = TextureImporterType.Default;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = true;
            ti.streamingMipmaps = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            maxSize = file == "S_Kelp" ? 1024 : 512;
        }
        else
        {
            ti.textureType = TextureImporterType.Default;
            ti.mipmapEnabled = !isSky;
            ti.streamingMipmaps = !isSky;
            ti.wrapMode = TextureWrapMode.Repeat;
            if (isSky) { ti.wrapModeV = TextureWrapMode.Clamp; maxSize = 2048; }
            else if (file == "T_Track_Sand" || file == "T_Seabed") maxSize = 1024;
        }

        var android = ti.GetPlatformTextureSettings("Android");
        android.overridden = true;
        android.maxTextureSize = maxSize;
        android.format = astc4 ? TextureImporterFormat.ASTC_4x4 : TextureImporterFormat.ASTC_6x6;
        ti.SetPlatformTextureSettings(android);
    }

    void OnPreprocessAudio()
    {
        if (!assetPath.StartsWith(Root)) return;
        var ai = (AudioImporter)assetImporter;
        string file = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        ai.forceToMono = true;
        var s = ai.defaultSampleSettings;
        if (file.StartsWith("MUS_"))
        {
            s.loadType = AudioClipLoadType.Streaming;           // music: streamed Vorbis 70
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = 0.7f;
            s.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            ai.loadInBackground = true;
        }
        else if (file.StartsWith("AMB_") || file == "SFX_NewBest")
        {
            s.loadType = AudioClipLoadType.CompressedInMemory;  // longer clips
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = 0.7f;
            s.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            s.sampleRateOverride = 22050;
        }
        else
        {
            s.loadType = AudioClipLoadType.DecompressOnLoad;    // short SFX under 1 s
            s.compressionFormat = AudioCompressionFormat.ADPCM;
            s.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            s.sampleRateOverride = 22050;
        }
        ai.defaultSampleSettings = s;
        ai.SetOverrideSampleSettings("Android", s);
    }

    void OnPreprocessModel()
    {
        if (!assetPath.StartsWith(Root)) return;
        var mi = (ModelImporter)assetImporter;
        mi.materialImportMode = ModelImporterMaterialImportMode.None;
        mi.importAnimation = false;
        mi.isReadable = false;
        mi.addCollider = false;
    }
}

public static class MinoUnderwaterSetup
{
    const string Root = "Assets/Art/Underwater/";
    const string MatDir = Root + "Materials";
    public static readonly Color FogColor = new Color(0.05f, 0.40f, 0.52f);

    // Scenery placement (edit if your track is wider, or move the UW_Scenery object afterwards)
    const float InnerX = 4.5f;    // closest to the track centre line (lanes are at -2, 0, 2)
    const float OuterX = 14f;
    const float StartZ = -20f;
    const float EndZ = 1260f;
    const float GroundY = 0f;

    [MenuItem("Mino/Underwater/1. Create Materials")]
    public static void CreateMaterials()
    {
        if (!AssetDatabase.IsValidFolder(MatDir)) AssetDatabase.CreateFolder("Assets/Art/Underwater", "Materials");
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) { Debug.LogError("[Mino] URP Lit shader not found"); return; }

        Lit(lit, "M_Track_Sand", "T_Track_Sand", new Vector2(1f, 40f), 0f, 0.15f, Color.black, false, false);
        Lit(lit, "M_Seabed", "T_Seabed", new Vector2(20f, 20f), 0f, 0.1f, Color.black, false, false);
        Lit(lit, "M_Obstacle_Coral", "T_Obstacle_Coral", Vector2.one, 0f, 0.35f, new Color(0.35f, 0.04f, 0.04f), false, false);
        Lit(lit, "M_Coin_Gold", "T_Coin_Gold", Vector2.one, 0.85f, 0.75f, new Color(0.45f, 0.30f, 0.05f), false, false);
        Lit(lit, "M_Rock", "T_Rock", Vector2.one, 0f, 0.2f, Color.black, false, true);
        Lit(lit, "M_Kelp", "S_Kelp", Vector2.one, 0f, 0.3f, Color.black, true, true);
        Lit(lit, "M_CoralFan", "S_CoralFan", Vector2.one, 0f, 0.3f, new Color(0.12f, 0.02f, 0.06f), true, true);

        var player = new Material(lit);
        player.SetColor("_BaseColor", new Color(1f, 0.55f, 0.12f));
        player.SetFloat("_Metallic", 0.2f);
        player.SetFloat("_Smoothness", 0.8f);
        player.EnableKeyword("_EMISSION");
        player.SetColor("_EmissionColor", new Color(0.25f, 0.10f, 0.0f));
        player.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        Save(player, "M_Player");

        var sky = new Material(Shader.Find("Skybox/Panoramic"));
        sky.SetTexture("_MainTex", Load<Texture2D>(Root + "Textures/T_Sky_Underwater_Panorama.png"));
        sky.SetFloat("_Mapping", 1f);
        sky.SetFloat("_ImageType", 0f);
        sky.SetFloat("_Exposure", 1f);
        Save(sky, "M_Sky_Underwater");

        AssignToPrefabs();
        AssetDatabase.SaveAssets();
        Debug.Log("[Mino] Underwater materials created in " + MatDir);
    }

    static void AssignToPrefabs()
    {
        var coral = Load<Material>(MatDir + "/M_Obstacle_Coral.mat");
        var gold = Load<Material>(MatDir + "/M_Coin_Gold.mat");
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.StartsWith(Root)) continue;
            var root = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;
            if (root.GetComponentInChildren<Obstacle>(true) != null) changed |= SetMat(root, coral);
            else if (root.GetComponentInChildren<Pickup>(true) != null) changed |= SetMat(root, gold);
            if (changed) { PrefabUtility.SaveAsPrefabAsset(root, path); Debug.Log("[Mino] material set on " + path); }
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static bool SetMat(GameObject root, Material m)
    {
        bool any = false;
        foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true)) { r.sharedMaterial = m; any = true; }
        return any;
    }

    [MenuItem("Mino/Underwater/2. Apply Lighting To Open Scene")]
    public static void ApplyLighting()
    {
        var sky = Load<Material>(MatDir + "/M_Sky_Underwater.mat");
        if (sky == null) { Debug.LogError("[Mino] Run 'Mino/Underwater/1. Create Materials' first"); return; }
        RenderSettings.skybox = sky;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogStartDistance = 25f;
        RenderSettings.fogEndDistance = 95f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.45f, 0.75f, 0.80f);
        RenderSettings.ambientEquatorColor = new Color(0.20f, 0.45f, 0.52f);
        RenderSettings.ambientGroundColor = new Color(0.08f, 0.18f, 0.22f);

        foreach (var l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            if (l.type == LightType.Directional) { l.color = new Color(0.75f, 0.95f, 1f); l.intensity = 1.1f; }
        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = FogColor;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Mino] Underwater fog, skybox and light applied. Save the scene (Ctrl+S).");
    }

    [MenuItem("Mino/Underwater/3. Spawn Scenery + Player Material (open scene)")]
    public static void SpawnScenery()
    {
        var rockMat = Load<Material>(MatDir + "/M_Rock.mat");
        var kelpMat = Load<Material>(MatDir + "/M_Kelp.mat");
        var fanMat = Load<Material>(MatDir + "/M_CoralFan.mat");
        if (rockMat == null) { Debug.LogError("[Mino] Run 'Mino/Underwater/1. Create Materials' first"); return; }

        var models = new[]
        {
            (Load<GameObject>(Root + "Models/Rock_A.obj"), rockMat, 0.8f, 1.8f, 3),
            (Load<GameObject>(Root + "Models/Rock_B.obj"), rockMat, 0.8f, 1.8f, 3),
            (Load<GameObject>(Root + "Models/Rock_C.obj"), rockMat, 0.6f, 1.4f, 3),
            (Load<GameObject>(Root + "Models/Kelp_Cross.obj"), kelpMat, 0.8f, 1.5f, 5),
            (Load<GameObject>(Root + "Models/CoralFan_Cross.obj"), fanMat, 0.7f, 1.3f, 3),
        };
        int totalWeight = 0;
        foreach (var m in models) { if (m.Item1 == null) { Debug.LogError("[Mino] Missing model in " + Root + "Models"); return; } totalWeight += m.Item5; }

        var old = GameObject.Find("UW_Scenery");
        if (old != null) Undo.DestroyObjectImmediate(old);
        var parent = new GameObject("UW_Scenery");
        Undo.RegisterCreatedObjectUndo(parent, "Spawn underwater scenery");

        var rng = new System.Random(1234);
        int count = 0;
        for (float z = StartZ; z < EndZ; z += 2.5f + (float)rng.NextDouble() * 3f)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                if (rng.NextDouble() < 0.25) continue;
                int pick = rng.Next(totalWeight), idx = 0;
                while (pick >= models[idx].Item5) { pick -= models[idx].Item5; idx++; }
                var (model, mat, minS, maxS, _) = models[idx];
                var go = (GameObject)PrefabUtility.InstantiatePrefab(model, parent.transform);
                float x = side * (InnerX + (float)rng.NextDouble() * (OuterX - InnerX));
                float s = minS + (float)rng.NextDouble() * (maxS - minS);
                go.transform.SetPositionAndRotation(new Vector3(x, GroundY, z + (float)rng.NextDouble()), Quaternion.Euler(0f, (float)rng.NextDouble() * 360f, 0f));
                go.transform.localScale = Vector3.one * s;
                foreach (var r in go.GetComponentsInChildren<MeshRenderer>()) { r.sharedMaterial = mat; r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; }
                GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
                count++;
            }
        }

        var playerMat = Load<Material>(MatDir + "/M_Player.mat");
        foreach (var runner in Object.FindObjectsByType<LaneRunner>(FindObjectsSortMode.None))
            foreach (var r in runner.GetComponentsInChildren<MeshRenderer>(true)) { Undo.RecordObject(r, "Player material"); r.sharedMaterial = playerMat; }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Mino] Spawned {count} scenery props under UW_Scenery (static batched). Move UW_Scenery on Y if they float or sink. Save the scene (Ctrl+S).");
    }

    static void Lit(Shader sh, string name, string tex, Vector2 tiling, float metal, float smooth, Color emission, bool cutout, bool doubleSided)
    {
        var m = new Material(sh);
        m.SetTexture("_BaseMap", Load<Texture2D>(Root + "Textures/" + tex + ".png"));
        m.SetColor("_BaseColor", Color.white);
        m.SetTextureScale("_BaseMap", tiling);
        m.SetFloat("_Metallic", metal);
        m.SetFloat("_Smoothness", smooth);
        if (emission != Color.black)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", emission);
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        }
        if (cutout)
        {
            m.SetFloat("_AlphaClip", 1f);
            m.SetFloat("_Cutoff", 0.5f);
            m.EnableKeyword("_ALPHATEST_ON");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        }
        if (doubleSided) m.SetFloat("_Cull", 0f);
        m.enableInstancing = true;
        Save(m, name);
    }

    static void Save(Material m, string name)
    {
        string path = MatDir + "/" + name + ".mat";
        var existing = Load<Material>(path);
        if (existing != null) { existing.shader = m.shader; existing.CopyPropertiesFromMaterial(m); EditorUtility.SetDirty(existing); }
        else AssetDatabase.CreateAsset(m, path);
    }

    static T Load<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);
}
