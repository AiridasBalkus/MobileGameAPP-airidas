using UnityEngine;

public class Pickup : PooledItem
{
    const float SpinSpeed = 120f; // degrees per second
    const float BobHeight = 0.15f;
    const float BobSpeed = 3f;
    float _baseY;

    void Reset() => GetComponent<Collider>().isTrigger = true;

    void OnEnable() => _baseY = transform.position.y; // pool sets the position before enabling

    protected override void Update()
    {
        base.Update();
        var p = transform.position;
        p.y = _baseY + Mathf.Sin(Time.time * BobSpeed + p.z) * BobHeight;
        transform.SetPositionAndRotation(p, Quaternion.Euler(0f, Time.time * SpinSpeed, 0f));
    }

    void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<PlayerCollision>();
        if (pickup == null) return;
        pickup.OnPickup();
        Pool.Release(this);
    }
}
