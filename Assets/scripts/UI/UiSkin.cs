using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Underwater UI sprites, applied on each scene load
public static class UiSkin
{
    const string SpritePath = "MinoUW/UI/";
    const float ButtonBorder = 48f, PanelBorder = 60f;
    static readonly Color32 PrimaryText = new Color32(0x3A, 0x1E, 0x05, 0xFF);

    static readonly string[] PrimaryNames = { "Play", "Retry" };
    const string PauseResumeParent = "PausePanel";

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
    static readonly List<Selectable> s_selectables = new List<Selectable>(32);
    static readonly Vector2 PrimaryMinSize = new Vector2(360f, 100f);
    const float PrimaryMinFont = 46f;
    const float MinTouchDp = 48f;

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
        Apply(SceneManager.GetActiveScene());
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

            root.GetComponentsInChildren(true, s_selectables);
            for (int i = 0; i < s_selectables.Count; i++) EnsureTouchTarget(s_selectables[i]);
            s_selectables.Clear();

            for (int i = 0; i < PanelPaths.Length; i++) SkinPanel(Find(root, PanelPaths[i]));
            if (scene.name == ResultScene)
            {
                SkinPanel(Find(root, ResultPanelPath));
                LightenDarkLabels(root); // dark text on the dark panel
                var buy = root.transform.Find("SafeArea/BuyCoinsButton") as RectTransform;
                if (buy) buy.anchoredPosition += new Vector2(0f, -30f); // room for the bigger Retry
            }

            if (root.name == "Canvas")
            {
                AddLogo(root.transform.Find("SafeArea/Title"), scene.name);
                AddCoinIcon(root.transform.Find("SafeArea/CoinsLabel"));
            }
        }
        s_buttons.Clear();
    }

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
        if (primary) img.rectTransform.sizeDelta = Vector2.Max(img.rectTransform.sizeDelta, PrimaryMinSize);
        img.pixelsPerUnitMultiplier = FitBorder(img.rectTransform, ButtonBorder);

        b.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++)
        {
            s_texts[i].color = primary ? (Color)PrimaryText : Color.white;
            if (primary) s_texts[i].fontSize = Mathf.Max(s_texts[i].fontSize, PrimaryMinFont);
        }
        s_texts.Clear();
    }

    // invisible hit pad so buttons and toggles are at least 48 dp, without changing the layout
    static void EnsureTouchTarget(Selectable sel)
    {
        if (!(sel is Button) && !(sel is Toggle)) return;
        if (sel.GetComponentInParent<ScrollRect>(true) || sel.transform.Find("HitPad")) return;
        var canvas = sel.GetComponentInParent<Canvas>(true);
        if (!canvas) return;
        float scale = canvas.rootCanvas.scaleFactor > 0f ? canvas.rootCanvas.scaleFactor : 1f;
        float minUnits = MinTouchDp * Mathf.Max(Screen.dpi, 160f) / 160f / scale;

        var rt = (RectTransform)sel.transform;
        Vector2 size = rt.rect.size;
        if (size.x >= minUnits && size.y >= minUnits) return;

        var go = new GameObject("HitPad", typeof(RectTransform), typeof(Image));
        var pad = (RectTransform)go.transform;
        pad.SetParent(rt, false);
        pad.SetAsFirstSibling();
        pad.anchorMin = pad.anchorMax = new Vector2(0.5f, 0.5f);
        pad.sizeDelta = new Vector2(Mathf.Max(size.x, minUnits), Mathf.Max(size.y, minUnits));
        var img = go.GetComponent<Image>();
        img.color = Color.clear;
        img.canvasRenderer.cullTransparentMesh = true;
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

    static float FitBorder(RectTransform rt, float border)
    {
        var r = rt.rect;
        float shortSide = Mathf.Min(r.width, r.height);
        return shortSide > 0f ? Mathf.Max(1f, 3f * border / shortSide) : 1f;
    }

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
        float width = 520f;
        float aspect = s_logo.rect.width / Mathf.Max(1f, s_logo.rect.height);
        rt.sizeDelta = new Vector2(width, width / aspect);
        var img = go.GetComponent<Image>();
        img.sprite = s_logo;
        img.preserveAspect = true;
        img.raycastTarget = false;
    }

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
