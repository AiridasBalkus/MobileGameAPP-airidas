using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] Feedback feedback;
    public void OnPickup() 
    { 
        gameManager.AddPickup();
        feedback.Pickup(); 
    }
    public void OnHitObstacle()
    {
        Haptics.Pulse();
        gameManager.Lose();
        feedback.Hit();
    }
}
