using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Bakes the underwater UI skin into the open scene so it can be edited in the Inspector
public static class MinoUiSkinBaker
{
    const string MenuPath = "Mino/Underwater/4. Bake UI Skin Into Open Scene";
    const string UndoName = "Bake UI Skin";
    static readonly string[] Folders =
    {
        "Assets/Art/Underwater/UI/",
        "Assets/Art/Underwater/Resources/MinoUW/UI/",
    };

    class Recorder : UiSkin.IRecorder
    {
        public readonly StringBuilder Log = new StringBuilder();
        public int Count;

        public void Record(Object target, string what)
        {
            Undo.RecordObject(target, UndoName);
            Log.Append("  ").Append(target.name).Append(": ").Append(what).Append('\n');
            Count++;
        }

        public void Created(GameObject go)
        {
            Undo.RegisterCreatedObjectUndo(go, UndoName);
            var parent = go.transform.parent;
            Log.Append("  ").Append(parent ? parent.name : "(root)").Append(": added ").Append(go.name).Append('\n');
            Count++;
        }
    }

    [MenuItem(MenuPath)]
    static void Bake()
    {
        var sprites = LoadSprites();
        if (sprites == null) return;

        var scene = EditorSceneManager.GetActiveScene();
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        var rec = new Recorder();

        foreach (var root in scene.GetRootGameObjects())
            UiSkin.Apply(root, scene.name, sprites, rec);
        FillSoundToggles(scene, sprites, rec);

        Undo.CollapseUndoOperations(group);
        if (rec.Count == 0)
        {
            Debug.Log("[UiSkin] " + scene.name + ": already baked, nothing changed");
            return;
        }
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[UiSkin] " + scene.name + ": " + rec.Count + " changes, save the scene\n" + rec.Log);
    }

    [MenuItem(MenuPath, true)]
    static bool BakeValidate() => !EditorApplication.isPlayingOrWillChangePlaymode;

    static void FillSoundToggles(UnityEngine.SceneManagement.Scene scene, UiSkin.Sprites sprites, Recorder rec)
    {
        foreach (var root in scene.GetRootGameObjects())
        foreach (var toggle in root.GetComponentsInChildren<SoundToggle>(true))
        {
            var iconT = toggle.transform.Find("Icon");
            var so = new SerializedObject(toggle);
            bool changed = Set(so, "icon", iconT ? iconT.GetComponent<Image>() : null)
                         | Set(so, "onIcon", sprites.SoundOn)
                         | Set(so, "offIcon", sprites.SoundOff);
            if (!changed) continue;
            so.ApplyModifiedProperties(); // records its own Undo
            rec.Log.Append("  ").Append(toggle.name).Append(": SoundToggle icon fields\n");
            rec.Count++;
        }
    }

    static bool Set(SerializedObject so, string field, Object value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.objectReferenceValue == value) return false;
        p.objectReferenceValue = value;
        return true;
    }

    static UiSkin.Sprites LoadSprites()
    {
        var s = new UiSkin.Sprites
        {
            Primary = Load("UI_Button_Primary"),
            Secondary = Load("UI_Button_Secondary"),
            Panel = Load("UI_Panel"),
            Logo = Load("UI_Logo_Mino"),
            Coin = Load("UI_Icon_Coin"),
            Pause = Load("Icon_Pause"),
            Play = Load("Icon_Play"),
            Settings = Load("Icon_Settings"),
            Upgrade = Load("Icon_Upgrade"),
            Vibrate = Load("Icon_Vibrate"),
            SoundOn = Load("Icon_SoundOn"),
            SoundOff = Load("Icon_SoundOff"),
            Font = LoadFont("Poppins-Bold SDF"),
        };
        if (!s.Primary || !s.Secondary || !s.Panel)
        {
            Debug.LogError("[UiSkin] button/panel sprites not found in " + string.Join(" or ", Folders));
            return null;
        }
        return s;
    }

    static Sprite Load(string name)
    {
        foreach (var folder in Folders)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(folder + name + ".png");
            if (sprite) return sprite;
        }
        Debug.LogWarning("[UiSkin] missing sprite " + name + " (is it imported as Sprite?)");
        return null;
    }

    static TMP_FontAsset LoadFont(string name)
    {
        foreach (var guid in AssetDatabase.FindAssets(name + " t:TMP_FontAsset"))
        {
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (font && font.name == name) return font;
        }
        return null;
    }
}
