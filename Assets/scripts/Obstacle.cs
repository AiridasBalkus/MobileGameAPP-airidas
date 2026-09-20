using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float despawnDistanceZ = -10f; //distance behind the player to despawn obstacles - AIRIDAS REMEBER TO CHANGE IT AND HAVE OBJECT POOOLING

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true; //make sure the collider is a trigger
    }
    private void OnTriggerEnter(Collider other)
    {
        var hit= other.GetComponent<PlayerCollision>();
        if (hit != null)
        {
            hit.OnHitObstacle();
        }
    }
    void Update()
    {
        if(transform.position.z < despawnDistanceZ)
        {
            Destroy(gameObject); //destroy the obstacle if it goes behind the player
        }
    }

}
