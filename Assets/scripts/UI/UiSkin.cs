using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Underwater UI skin mapping, baked into scenes by Mino/Underwater/4 (works in edit and play mode)
public static class UiSkin
{
    public class Sprites
    {
        public Sprite Primary, Secondary, Panel, Logo, Coin;
        public Sprite Pause, Play, Settings, Upgrade, Vibrate, SoundOn, SoundOff;
        public TMP_FontAsset Font; // optional
    }

    // editor passes one that records Undo and logs; null at runtime
    public interface IRecorder
    {
        void Record(Object target, string what);
        void Created(GameObject go);
    }

    const float ButtonBorder = 48f, PanelBorder = 60f;
    public static readonly Color32 PrimaryText = new Color32(0x3A, 0x1E, 0x05, 0xFF);

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

    enum IconPlace { Left, Center, Right }
    static readonly List<Button> s_buttons = new List<Button>(32);
    static readonly List<TMP_Text> s_texts = new List<TMP_Text>(4);
    static readonly List<Selectable> s_selectables = new List<Selectable>(32);
    static readonly Vector2 PrimaryMinSize = new Vector2(360f, 100f);
    const float PrimaryMinFont = 46f;
    const float MinTouchDp = 48f, PhoneWidthDp = 360f;

    static Sprites s;
    static IRecorder rec;

    public static void Apply(GameObject root, string sceneName, Sprites sprites, IRecorder recorder = null)
    {
        s = sprites;
        rec = recorder;

        // before icons and hit pads so the clone doesn't copy them
        if (root.name == "Canvas") AddSoundToggle(root.transform);

        if (s.Font)
        {
            root.GetComponentsInChildren(true, s_texts);
            for (int i = 0; i < s_texts.Count; i++)
                if (s_texts[i].font != s.Font) { Rec(s_texts[i], "font"); s_texts[i].font = s.Font; }
            s_texts.Clear();
        }

        root.GetComponentsInChildren(true, s_buttons);
        if (sceneName == ResultScene) MoveBuyCoins(root);
        for (int i = 0; i < s_buttons.Count; i++) SkinButton(s_buttons[i]);
        s_buttons.Clear();

        root.GetComponentsInChildren(true, s_selectables);
        for (int i = 0; i < s_selectables.Count; i++) EnsureTouchTarget(s_selectables[i]);
        s_selectables.Clear();

        for (int i = 0; i < PanelPaths.Length; i++) SkinPanel(Find(root, PanelPaths[i]));
        if (sceneName == ResultScene)
        {
            SkinPanel(Find(root, ResultPanelPath));
            LightenDarkLabels(root); // dark text on the dark panel
        }

        if (root.name == "Canvas")
        {
            AddLogo(root.transform.Find("SafeArea/Title"), sceneName);
            AddCoinIcon(root.transform.Find("SafeArea/CoinsLabel"));
            AddIcons(root.transform, sceneName);
        }
        s = null;
        rec = null;
    }

    static void Rec(Object o, string what) { if (rec != null) rec.Record(o, what); }
    static void Made(GameObject go) { if (rec != null) rec.Created(go); }

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

    // Sound toggle is a copy of the haptics toggle
    static void AddSoundToggle(Transform canvas)
    {
        var haptics = canvas.Find("SafeArea/SettingsPanel/Toggle");
        if (!haptics || haptics.parent.Find("SoundToggle")) return;

        var clone = Object.Instantiate(haptics.gameObject, haptics.parent);
        clone.name = "SoundToggle";
        var old = clone.GetComponent<HapticsToggle>();
        if (old) Object.DestroyImmediate(old);
        var icon = clone.transform.Find("Icon");
        if (icon) Object.DestroyImmediate(icon.gameObject);
        var pad = clone.transform.Find("HitPad");
        if (pad) Object.DestroyImmediate(pad.gameObject);
        clone.AddComponent<SoundToggle>();

        var rt = (RectTransform)clone.transform;
        var src = (RectTransform)haptics;
        rt.anchoredPosition = src.anchoredPosition + new Vector2(0f, src.rect.height + 40f);
        var label = clone.GetComponentInChildren<TMP_Text>(true);
        if (label) label.text = "Sound";
        Made(clone);
    }

    // room for the bigger Retry, only on the first bake
    static void MoveBuyCoins(GameObject root)
    {
        var buy = root.transform.Find("SafeArea/BuyCoinsButton") as RectTransform;
        if (!buy) return;
        for (int i = 0; i < s_buttons.Count; i++)
        {
            var b = s_buttons[i];
            if (b.name != "Retry" || !b.image) continue;
            if (b.image.sprite == s.Primary) return;
            Rec(buy, "moved down 30");
            buy.anchoredPosition += new Vector2(0f, -30f);
            return;
        }
    }

    static void SkinButton(Button b)
    {
        var img = b.image;
        if (!img) return;
        bool primary = IsPrimary(b.transform);
        var sprite = primary ? s.Primary : s.Secondary;
        if (!sprite) return;

        var rt = img.rectTransform;
        Vector2 size = primary ? Vector2.Max(rt.sizeDelta, PrimaryMinSize) : rt.sizeDelta;
        if (size != rt.sizeDelta) { Rec(rt, "size " + size); rt.sizeDelta = size; }
        SetSliced(img, sprite, ButtonBorder, primary ? "primary button" : "secondary button");

        Color textColor = primary ? (Color)PrimaryText : Color.white;
        b.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++)
        {
            var t = s_texts[i];
            float fontSize = primary ? Mathf.Max(t.fontSize, PrimaryMinFont) : t.fontSize;
            if (t.color == textColor && t.fontSize == fontSize) continue;
            Rec(t, "text colour/size");
            t.color = textColor;
            t.fontSize = fontSize;
        }
        s_texts.Clear();
    }

    static void SkinPanel(Transform t)
    {
        if (!t || !s.Panel) return;
        var img = t.GetComponent<Image>();
        if (img) SetSliced(img, s.Panel, PanelBorder, "panel");
    }

    static void SetSliced(Image img, Sprite sprite, float border, string what)
    {
        float ppu = FitBorder(img.rectTransform, border);
        if (img.sprite == sprite && img.type == Image.Type.Sliced && img.color == Color.white
            && Mathf.Approximately(img.pixelsPerUnitMultiplier, ppu)) return;
        Rec(img, what + " sprite");
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.color = Color.white;
        img.pixelsPerUnitMultiplier = ppu;
    }

    static float FitBorder(RectTransform rt, float border)
    {
        var r = rt.rect;
        float shortSide = Mathf.Min(r.width, r.height);
        return shortSide > 0f ? Mathf.Max(1f, 3f * border / shortSide) : 1f;
    }

    // 48 dp in canvas units, for the narrowest phone (360 dp wide)
    static float MinTouchUnits(Canvas canvas)
    {
        var scaler = canvas.rootCanvas.GetComponent<CanvasScaler>();
        if (scaler && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            return scaler.referenceResolution.x * MinTouchDp / PhoneWidthDp;
        return MinTouchDp * 2.625f; // ~420 dpi
    }

    // invisible hit pad so buttons and toggles are at least 48 dp, without changing the layout
    static void EnsureTouchTarget(Selectable sel)
    {
        if (!(sel is Button) && !(sel is Toggle)) return;
        if (sel.GetComponentInParent<ScrollRect>(true) || sel.transform.Find("HitPad")) return;
        var canvas = sel.GetComponentInParent<Canvas>(true);
        if (!canvas) return;
        float minUnits = MinTouchUnits(canvas);

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
        Made(go);
    }

    static void LightenDarkLabels(GameObject root)
    {
        root.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++)
        {
            var t = s_texts[i];
            if (t.GetComponentInParent<Selectable>(true)) continue;
            Color c = t.color;
            if (0.299f * c.r + 0.587f * c.g + 0.114f * c.b >= 0.5f) continue;
            Rec(t, "text colour white");
            t.color = new Color(1f, 1f, 1f, c.a);
        }
        s_texts.Clear();
    }

    static void AddLogo(Transform title, string sceneName)
    {
        if (sceneName != "Menu" || !title || !s.Logo) return;
        var text = title.GetComponent<TMP_Text>();
        if (text && text.enabled) { Rec(text, "hidden for logo"); text.enabled = false; }
        if (title.parent.Find("Logo")) return;

        var go = new GameObject("Logo", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(title.parent, false);
        rt.SetSiblingIndex(title.GetSiblingIndex());
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -40f);
        float width = 520f;
        float aspect = s.Logo.rect.width / Mathf.Max(1f, s.Logo.rect.height);
        rt.sizeDelta = new Vector2(width, width / aspect);
        var img = go.GetComponent<Image>();
        img.sprite = s.Logo;
        img.preserveAspect = true;
        img.raycastTarget = false;
        Made(go);
    }

    static void AddCoinIcon(Transform coins)
    {
        if (!coins || !s.Coin || coins.Find("CoinIcon")) return;
        var text = coins.GetComponent<TMP_Text>();
        if (text) { Rec(text, "left aligned"); text.horizontalAlignment = HorizontalAlignmentOptions.Left; }

        var go = new GameObject("CoinIcon", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(coins, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-12f, 0f);
        rt.sizeDelta = new Vector2(90f, 90f);
        var img = go.GetComponent<Image>();
        img.sprite = s.Coin;
        img.preserveAspect = true;
        img.raycastTarget = false;
        Made(go);
    }

    static void AddIcons(Transform canvas, string sceneName)
    {
        Color dark = PrimaryText;
        if (sceneName == "Menu") AddIcon(canvas.Find("SafeArea/Play"), s.Play, dark, IconPlace.Left);
        if (sceneName != "Game") return;
        AddIcon(canvas.Find("SafeArea/Pause"), s.Pause, Color.white, IconPlace.Center);
        AddIcon(canvas.Find("SafeArea/PausePanel/Resume"), s.Play, dark, IconPlace.Left);
        AddIcon(canvas.Find("SafeArea/PausePanel/Settings"), s.Settings, Color.white, IconPlace.Left);
        AddIcon(canvas.Find("SafeArea/UpgradeButton"), s.Upgrade, Color.white, IconPlace.Left);
        AddIcon(canvas.Find("SafeArea/SettingsPanel/Toggle"), s.Vibrate, Color.white, IconPlace.Right);
        AddIcon(canvas.Find("SafeArea/SettingsPanel/SoundToggle"), s.SoundOn, Color.white, IconPlace.Right);
    }

    // Left: inside the button, text moves right. Center: replaces the text. Right: just outside the row.
    static void AddIcon(Transform target, Sprite sprite, Color color, IconPlace place)
    {
        if (!target || !sprite || target.Find("Icon")) return;
        var host = (RectTransform)target;
        Vector2 size = host.rect.size;
        float icon = Mathf.Min(size.y * (place == IconPlace.Center ? 0.6f : 0.55f), 72f);
        const float pad = 16f;

        var go = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(host, false);
        rt.sizeDelta = new Vector2(icon, icon);
        if (place == IconPlace.Center)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        }
        else if (place == IconPlace.Left)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(pad, 0f);
        }
        else
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(pad, 0f);
        }
        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.preserveAspect = true;
        img.raycastTarget = false;
        Made(go);

        if (place == IconPlace.Right) return;
        target.GetComponentsInChildren(true, s_texts);
        for (int i = 0; i < s_texts.Count; i++)
        {
            var t = s_texts[i];
            if (place == IconPlace.Center) { Rec(t, "hidden for icon"); t.enabled = false; continue; }
            var trt = t.rectTransform;
            if (trt.anchorMin.x != 0f || trt.anchorMax.x != 1f) continue;
            Rec(trt, "text moved right for icon");
            trt.offsetMin += new Vector2(icon + pad, 0f);
        }
        s_texts.Clear();
    }
}
