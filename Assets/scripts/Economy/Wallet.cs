using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    public int Coins { get; private set; }
    public int Level { get; private set; }
    public event System.Action Changed;
    public int NextCost => (int)(100 * Mathf.Pow(1.5f, Level));

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Earn(int n) { Coins += n; Changed?.Invoke(); }

    public void Restore(int c, int l) { Coins = c; Level = l; Changed?.Invoke(); }

    public bool TryUpgrade()
    {
        int cost = NextCost;
        if (Coins < cost) return false;
        Coins -= cost; Level++;
        Telemetry.Log("upgrade_purchased", ("upgrade_id", "speed"), ("level", Level),
            ("cost", cost), ("coins_left", Coins));
        Changed?.Invoke();
        return true;
    }
}
