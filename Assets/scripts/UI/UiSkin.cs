using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Applies the underwater UI sprites (Resources/MinoUW/UI) to every scene as it loads.
// Runs once per scene load, never per frame. No Inspector wiring.
public static class UiSkin
{
    const string SpritePath = "MinoUW/UI/";
    const float ButtonBorder = 48f, PanelBorder = 60f;   // sprite 9-slice borders (px)
    static readonly Color32 PrimaryText = new Color32(0x3A, 0x1E, 0x05, 0xFF);

    // Main action per screen, matched by GameObject name (+ parent for Resume).
    static readonly string[] PrimaryNames = { "Play", "Retry" };
    const string PauseResumeParent = "PausePanel";

    // Panels, as paths from a root object: Game pause/settings, Menu About, Result panel, Boot loading screen.
    static readonly string[] PanelPaths =
    {
        "Canvas/SafeArea/PausePanel",
        "Canvas/SafeArea/SettingsPanel",
        "Canvas/SafeArea/AboutPanel",
        "App/LoadingCanvas/LoadingScreen",
    };
    const string ResultScene = "Result", ResultPanelPath = "Canvas/SafeArea/Panel";

    static Sprite s_primary, s_secondary, s_panel, s_logo, s_coin;
    static readonly List<Button> s_buttons = new List<Button>(32);
    static readonly List<TMP_Text> s_texts = new List<TMP_Text>(4);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        s_primary = Load("UI_Button_Primary");
        s_secondary = Load("UI_Button_Secondary");
        s_panel = Load("UI_Panel");
        s_logo = Load("UI_Logo_Mino");
        s_coin = Load("UI_Icon_Coin");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Apply(SceneManager.GetActiveScene());   // Boot (already loaded when this runs)
    }

    static Sprite Load(string name)
    {
        var s = Resources.Load<Sprite>(SpritePath + name);
        if (!s) Debug.LogWarning("[UiSkin] missing sprite Resources/" + SpritePath + name);
        return s;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Apply(scene);

    static void Apply(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            root.GetComponentsInChildren(true, s_buttons);
            for (int i = 0; i < s_buttons.Count; i++) SkinButton(s_buttons[i]);

            for (int i = 0; i < PanelPaths.Length; i++) SkinPanel(Find(root, PanelPaths[i]));
            if (scene.name == ResultScene)
            {
                SkinPanel(Find(root, ResultPanelPath));
                LightenDarkLabels(root);   // Score/Best/Title were black on the old light background
            }

            if (root.name == "Canvas")
            {
                AddLogo(root.transform.Find("SafeArea/Title"), scene.name);
                AddCoinIcon(root.transform.Find("SafeArea/CoinsLabel"));
            }
        }
        s_buttons.Clear();
    }

    // "Canvas/SafeArea/X" -> the root must be named Canvas, then Find the rest.
    static Transform Find(GameObject root, string path)
    {
        int slash = path.IndexOf('/');
        if (slash < 0 || string.CompareOrdinal(root.name, 0, path, 0, slash) != 0 || root.name.Length != slash) return null;
        return root.transform.Find(path.Substring(slash + 1));
    }

    static bool IsPrimary(Transform t)
    {
        for (int i = 0; i < PrimaryNames.Length; i++) if (t.name == PrimaryNames[i]) return true;
        return t.name == "Resume" && t.parent && t.parent.name == PauseResumeParent;
    }

    static void SkinButton(Button b)
    {
        var img = b.image;
        if (!img) return;
        bool primary = IsPrimary(b.transform);
        var sprite = primary ? s_primary : s_secondary;
        if (!sprite) return;
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.color = Color.white;
        img.pixelsPerUnitMultiplier = FitBorder(img.rectTransform, ButtonBorder);

        b.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++) s_texts[i].color = primary ? (Color)PrimaryText : Color.white;
        s_texts.Clear();
    }

    static void SkinPanel(Transform t)
    {
        if (!t || !s_panel) return;
        var img = t.GetComponent<Image>();
        if (!img) return;
        img.sprite = s_panel;
        img.type = Image.Type.Sliced;
        img.color = Color.white;
        img.pixelsPerUnitMultiplier = FitBorder(img.rectTransform, PanelBorder);
    }

    // Non-button labels that are dark become white so they read on the dark UI_Panel.
    static void LightenDarkLabels(GameObject root)
    {
        root.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++)
        {
            var t = s_texts[i];
            if (t.GetComponentInParent<Selectable>(true)) continue;
            Color c = t.color;
            if (0.299f * c.r + 0.587f * c.g + 0.114f * c.b < 0.5f) t.color = new Color(1f, 1f, 1f, c.a);
        }
        s_texts.Clear();
    }

    // Keeps each 9-slice border at most a third of the element's short side.
    static float FitBorder(RectTransform rt, float border)
    {
        var r = rt.rect;
        float shortSide = Mathf.Min(r.width, r.height);
        return shortSide > 0f ? Mathf.Max(1f, 3f * border / shortSide) : 1f;
    }

    // Menu: hide the "Mino" title text and show the logo in its place (top centre).
    static void AddLogo(Transform title, string sceneName)
    {
        if (sceneName != "Menu" || !title || !s_logo || title.parent.Find("Logo")) return;
        var text = title.GetComponent<TMP_Text>();
        if (text) text.enabled = false;

        var go = new GameObject("Logo", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(title.parent, false);
        rt.SetSiblingIndex(title.GetSiblingIndex());
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -40f);
        float width = 520f;                                // Menu canvas is 800 units wide
        float aspect = s_logo.rect.width / Mathf.Max(1f, s_logo.rect.height);
        rt.sizeDelta = new Vector2(width, width / aspect);
        var img = go.GetComponent<Image>();
        img.sprite = s_logo;
        img.preserveAspect = true;
        img.raycastTarget = false;
    }

    // Game HUD: coin icon just left of the coin number (number left-aligned so it sits next to it).
    static void AddCoinIcon(Transform coins)
    {
        if (!coins || !s_coin || coins.Find("CoinIcon")) return;
        var text = coins.GetComponent<TMP_Text>();
        if (text) text.horizontalAlignment = HorizontalAlignmentOptions.Left;

        var go = new GameObject("CoinIcon", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(coins, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-12f, 0f);
        rt.sizeDelta = new Vector2(90f, 90f);
        var img = go.GetComponent<Image>();
        img.sprite = s_coin;
        img.preserveAspect = true;
        img.raycastTarget = false;
    }
}
