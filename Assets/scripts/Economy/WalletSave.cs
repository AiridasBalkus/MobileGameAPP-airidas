using UnityEngine;

public static class WalletSave
{
    const string CoinsKey = "wallet.coins", LevelKey = "wallet.level";

    public static void Save(Wallet w)
    {
        PlayerPrefs.SetInt(CoinsKey, w.Coins);
        PlayerPrefs.SetInt(LevelKey, w.Level);
        PlayerPrefs.Save();
    }

    public static void Load(Wallet w) =>
        w.Restore(PlayerPrefs.GetInt(CoinsKey, 0), PlayerPrefs.GetInt(LevelKey, 0));
    public static void Reset(Wallet w)
    {
        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.Save();
        w.Restore(0, 0);
    }
}
