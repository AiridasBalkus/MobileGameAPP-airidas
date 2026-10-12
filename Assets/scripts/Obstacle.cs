using UnityEngine;

public class Obstacle : PooledItem
{
    void Reset() => GetComponent<Collider>().isTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        var p = other.GetComponent<PlayerCollision>();
        if (p != null) p.OnHitObstacle();
    }
}
