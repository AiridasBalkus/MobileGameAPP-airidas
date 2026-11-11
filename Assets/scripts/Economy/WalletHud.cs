using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WalletHud : MonoBehaviour
{
    [SerializeField] TMP_Text coinsLabel;
    [SerializeField] TMP_Text upgradeLabel;
    [SerializeField] Button upgradeButton;

    // W09: peers asked "what does upgrade do?", so the label names the effect and level.
    const string UpgradeFormat = "Faster lane change  Lv {0}\n{1} coins";
    const float DisabledTextAlpha = 0.45f;

    Wallet wallet;

    void Awake()
    {
        // Two lines must fit the existing button rect: shrink to fit, never grow past the scene size.
        upgradeLabel.enableAutoSizing = true;
        upgradeLabel.fontSizeMax = upgradeLabel.fontSize;
        upgradeLabel.fontSizeMin = 18f;
    }

    void OnEnable()
    {
        wallet = Wallet.Instance;
        if (wallet == null) { Debug.LogWarning("[Wallet] none: start Play from Boot"); return; }
        wallet.Changed += Refresh;
        Refresh();
    }

    void OnDisable() { if (wallet != null) wallet.Changed -= Refresh; }

    // Event-driven only (coins or level change), never per frame.
    void Refresh()
    {
        coinsLabel.SetText("{0}", wallet.Coins);
        int cost = wallet.NextCost;
        bool canAfford = wallet.Coins >= cost;
        upgradeLabel.SetText(UpgradeFormat, (float)wallet.Level, (float)cost);
        upgradeLabel.alpha = canAfford ? 1f : DisabledTextAlpha;
        upgradeButton.interactable = canAfford;
    }

    public void OnUpgrade()
    {
        if (wallet == null) return;
        if (wallet.TryUpgrade()) Debug.Log($"[Wallet] upgraded to level {wallet.Level} at {Time.timeSinceLevelLoad:F0}s");
        else Debug.Log("[Wallet] not enough coins");
    }
}
