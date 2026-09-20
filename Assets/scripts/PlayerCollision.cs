using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public void OnHitObstacle()
    {
        Haptics.Pulse();
        Debug.Log("[HIT]Player hit an obstacle!");
    }
}
