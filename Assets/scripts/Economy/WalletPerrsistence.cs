using UnityEngine;

[RequireComponent(typeof(Wallet))]
public class WalletPerrsistence : MonoBehaviour
{
    Wallet wallet;

    void Awake()
    {
        wallet = GetComponent<Wallet>();
        WalletSave.Load(wallet);
        wallet.Changed += Save;
        Debug.Log($"[Wallet] loaded {wallet.Coins} coins, level {wallet.Level}");
    }

    void OnDestroy() { if (wallet) wallet.Changed -= Save; }

    void Save() => WalletSave.Save(wallet);

    void OnApplicationPause(bool paused) { if (paused) Save(); }
}
