using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WalletHud : MonoBehaviour
{
    [SerializeField] TMP_Text coinsLabel;
    [SerializeField] TMP_Text upgradeLabel;
    [SerializeField] Button upgradeButton;

    const string UpgradeFormat = "Faster lane change  Lv {0}\n{1} coins";
    const float DisabledTextAlpha = 0.45f;
    const float PopTime = 0.6f;
    const float PopScale = 1.25f;

    Wallet wallet;
    TMP_Text _gainLabel;
    int _lastCoins = -1;
    float _popT = PopTime; // >= PopTime means idle

    void Awake()
    {
        // shrink to fit the button
        upgradeLabel.enableAutoSizing = true;
        upgradeLabel.fontSizeMax = upgradeLabel.fontSize;
        upgradeLabel.fontSizeMin = 18f;
        _gainLabel = CreateGainLabel();
    }

    void OnEnable()
    {
        wallet = Wallet.Instance;
        if (wallet == null) { Debug.LogWarning("[Wallet] none: start Play from Boot"); return; }
        wallet.Changed += Refresh;
        Refresh();
    }

    void OnDisable() { if (wallet != null) wallet.Changed -= Refresh; }

    void Update()
    {
        if (_popT >= PopTime) return;
        _popT += Time.deltaTime;
        float k = Mathf.Clamp01(_popT / PopTime);
        float pop = k < 0.25f ? Mathf.Lerp(1f, PopScale, k / 0.25f) : Mathf.Lerp(PopScale, 1f, (k - 0.25f) / 0.75f);
        coinsLabel.rectTransform.localScale = new Vector3(pop, pop, 1f);
        _gainLabel.alpha = 1f - k;
        _gainLabel.rectTransform.anchoredPosition = new Vector2(0f, 20f + 60f * k);
    }

    void Refresh()
    {
        coinsLabel.SetText("{0}", wallet.Coins);
        int gained = wallet.Coins - _lastCoins;
        if (_lastCoins >= 0 && gained > 0)
        {
            _gainLabel.SetText("+{0}", gained);
            _popT = 0f;
        }
        _lastCoins = wallet.Coins;
        int cost = wallet.NextCost;
        bool canAfford = wallet.Coins >= cost;
        upgradeLabel.SetText(UpgradeFormat, (float)wallet.Level, (float)cost);
        upgradeLabel.alpha = canAfford ? 1f : DisabledTextAlpha;
        upgradeButton.interactable = canAfford;
    }

    public void OnUpgrade()
    {
        if (wallet == null) return;
        if (wallet.TryUpgrade())
        {
            var audio = AudioService.Instance;
            if (audio) audio.Upgrade();
            DevLog.Log($"[Wallet] upgraded to level {wallet.Level} at {Time.timeSinceLevelLoad:F0}s");
        }
        else DevLog.Log("[Wallet] not enough coins");
    }

    // "+5" above the coin counter
    TMP_Text CreateGainLabel()
    {
        var go = new GameObject("CoinGain", typeof(RectTransform), typeof(TextMeshProUGUI));
        var rt = (RectTransform)go.transform;
        rt.SetParent(coinsLabel.rectTransform, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(200f, 70f);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 50f;
        t.color = new Color(1f, 0.85f, 0.2f);
        t.raycastTarget = false;
        t.alpha = 0f;
        return t;
    }
}
