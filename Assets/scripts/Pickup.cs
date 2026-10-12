using UnityEngine;

public class Pickup : PooledItem
{
    void Reset() => GetComponent<Collider>().isTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<PlayerCollision>();
        if (pickup == null) return;
        pickup.OnPickup();
        Pool.Release(this);
    }
}