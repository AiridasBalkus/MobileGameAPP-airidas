using JetBrains.Annotations;
using System.Linq.Expressions;
using UnityEngine;

public class LaneRunner : MonoBehaviour
{
    public float laneWidth = 2.0f; // Width of each lane
    public int laneCount = 3;
    public float laneChangeSpeed = 12.0f; // Speed of lane change

    public float fowardSpeed = 5.0f; //cosntant Forward movement speed

    int _currentLane;
    float _targetXPosition;

    void Start()
    {
        _currentLane = 0; // Start in the middle lane
        _targetXPosition = 0f;
    }

    void Update()
    {
        transform.position += Vector3.forward * fowardSpeed * Time.deltaTime; // Move forward constantly

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
    }

}
