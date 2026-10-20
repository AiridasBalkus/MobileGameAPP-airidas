using UnityEngine;

public class FakePurchaseProvider : IPurchaseProvider
{
    public async Awaitable<bool> PurchaseAsync(string productId)
    {
        Telemetry.Log("purchase_stub", ("product_id", productId));
        Debug.Log($"[iap-stub] purchase {productId}");
        await Awaitable.WaitForSecondsAsync(0.5f);
        return true;
    }
}
