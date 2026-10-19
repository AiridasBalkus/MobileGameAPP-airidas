using UnityEngine;
using UnityEngine.UI;

public class CoinShop : MonoBehaviour
{
    [SerializeField] Button buyButton;
    IPurchaseProvider store = new FakePurchaseProvider();
    bool _busy;

    public async void OnBuyPack()
    {
        if (_busy || !Wallet.Instance) return;
        _busy = true; buyButton.interactable = false;
        try
        {
            if (await store.PurchaseAsync("coins_500"))
            {
                Wallet.Instance.Earn(500);
                Debug.Log($"[iap-stub] granted 500, coins now {Wallet.Instance.Coins}");
            }
        }
        finally
        {
            _busy = false;
            if (buyButton) buyButton.interactable = true;
        }
    }
}
