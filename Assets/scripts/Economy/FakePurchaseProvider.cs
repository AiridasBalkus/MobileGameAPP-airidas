using UnityEngine;

public class FakePurchaseProvider : IPurchaseProvider
{
    public async Awaitable<bool> PurchaseAsync(string productId)
    {
        Debug.Log($"[iap-stub] purchase {productId}");
        await Awaitable.WaitForSecondsAsync(0.5f);
        return true;
    }
}
