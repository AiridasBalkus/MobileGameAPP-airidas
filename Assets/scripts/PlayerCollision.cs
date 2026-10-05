using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public void OnHitObstacle()
    {
        Haptics.Pulse();
        gameManager.Lose();
    }
}
