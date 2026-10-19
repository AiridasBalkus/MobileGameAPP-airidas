using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WalletHud : MonoBehaviour
{
    [SerializeField] TMP_Text coinsLabel;
    [SerializeField] TMP_Text upgradeLabel;
    [SerializeField] Button upgradeButton;

    Wallet wallet;

    void OnEnable()
    {
        wallet = Wallet.Instance;
        if (wallet == null) { Debug.LogWarning("[Wallet] none: start Play from Boot"); return; }
        wallet.Changed += Refresh;
        Refresh();
    }

    void OnDisable() { if (wallet != null) wallet.Changed -= Refresh; }

    void Refresh()
    {
        coinsLabel.SetText("{0}", wallet.Coins);
        upgradeLabel.SetText("Upgrade ({0})", wallet.NextCost);
        upgradeButton.interactable = wallet.Coins >= wallet.NextCost;
    }

    public void OnUpgrade()
    {
        if (wallet == null) return;
        if (wallet.TryUpgrade()) Debug.Log($"[Wallet] upgraded to level {wallet.Level} at {Time.timeSinceLevelLoad:F0}s");
        else Debug.Log("[Wallet] not enough coins");
    }
}
