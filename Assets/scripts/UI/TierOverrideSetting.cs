using TMPro;
using UnityEngine;

// Put on a TMP Dropdown with options Auto, Low, Mid, High
public class TierOverrideSetting : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] TMP_Text note;

    static readonly string[] Notes =
    {
        "Auto: tier picked from this phone. Applies on next launch.",
        "Low tier saved. Applies on next launch.",
        "Mid tier saved. Applies on next launch.",
        "High tier saved. Applies on next launch.",
    };

    void Awake()
    {
        if (!dropdown) dropdown = GetComponent<TMP_Dropdown>();
        if (!note) note = CreateNote();
    }

    void OnEnable()
    {
        int option = SavedOption();
        if (dropdown)
        {
            dropdown.SetValueWithoutNotify(option);
            dropdown.onValueChanged.AddListener(SetTierOverride);
        }
        ShowNote(option);
    }

    void OnDisable() { if (dropdown) dropdown.onValueChanged.RemoveListener(SetTierOverride); }

    // 0 = Auto, 1..3 = Low..High
    public void SetTierOverride(int option)
    {
        if (option <= 0) PlayerPrefs.DeleteKey(DeviceTier.OverrideKey);
        else PlayerPrefs.SetInt(DeviceTier.OverrideKey, Mathf.Clamp(option - 1, DeviceTier.Low, DeviceTier.High));
        PlayerPrefs.Save();
        ShowNote(option);
        Debug.Log($"[perf] tier_override option={option} (current tier {DeviceTier.CurrentName} until relaunch)");
    }

    static int SavedOption()
    {
        int stored = PlayerPrefs.GetInt(DeviceTier.OverrideKey, -1);
        return stored >= DeviceTier.Low && stored <= DeviceTier.High ? stored + 1 : 0;
    }

    void ShowNote(int option)
    {
        if (note) note.SetText(Notes[Mathf.Clamp(option, 0, Notes.Length - 1)]);
    }

    TMP_Text CreateNote()
    {
        var go = new GameObject("TierNote", typeof(RectTransform), typeof(TextMeshProUGUI));
        var rt = (RectTransform)go.transform;
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -8f);
        rt.sizeDelta = new Vector2(120f, 70f);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.white;
        t.raycastTarget = false;
        t.enableAutoSizing = true;
        t.fontSizeMin = 18f;
        t.fontSizeMax = 30f;
        return t;
    }
}
