using UnityEngine;

public class LaneRunner : MonoBehaviour
{
    [SerializeField] Feedback feedback;
    public float laneWidth = 2.0f; // Width of each lane
    public int laneCount = 3;
    public float laneChangeSpeed = 12.0f; // Speed of lane change
    public float startSpeed = 5f;
    public float maxSpeed = 12f;
    public float acceleration = 0.05f;
    int _currentLane;
    float _targetXPosition;
    float _speed;
    public float Speed => _speed;

    void Start()
    {
        _currentLane = 0; // Start in the middle lane
        _targetXPosition = 0f;
        _speed = startSpeed;
    }

    void Update()
    {
       _speed = Mathf.Min(maxSpeed, _speed + acceleration * Time.deltaTime);
        transform.position += Vector3.forward * _speed * Time.deltaTime; //Move forward

        Vector3 position = transform.position; // Get current position
        position.x = Mathf.MoveTowards(position.x, _targetXPosition, laneChangeSpeed * Time.deltaTime); //smoothly move towards the target lane position
        transform.position = position; //update the position
    }

    public void ChangeLane(int direction)
    {
        int half = laneCount /2; // Calculate half the lane count for clamping
        int newLane = Mathf.Clamp(_currentLane + direction, -half, half);
        if (newLane == _currentLane) return;
        _currentLane = newLane;
        _targetXPosition = _currentLane * laneWidth;
        if (feedback) feedback.LaneChange();
    }

}
