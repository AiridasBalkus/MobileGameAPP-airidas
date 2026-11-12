using UnityEngine;

public class WalletResetButton : MonoBehaviour
{
    public void OnReset() { if (Wallet.Instance) WalletSave.Reset(Wallet.Instance); DevLog.Log("[Wallet] reset"); }
}
