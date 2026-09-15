using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class tapSwipeInput : MonoBehaviour
{

    public float swipeDp = 50f; // Minimum swipe distance in dp
    public float tapMaxTime = 0.3f; // Maximum time for a tap in seconds

    public LaneRunner runner;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase != TouchPhase.Ended) continue;
            float px = swipeDp * Mathf.Max(Screen.dpi, 160f) / 160f;
            Vector2 d = touch.screenPosition - touch.startScreenPosition;

            if (d.magnitude >= px)
            {
                // Determine if the swipe is primarily horizontal or vertical
                if (Mathf.Abs(d.x) >= Mathf.Abs(d.y))
                {
                    if (runner != null)
                        runner.ChangeLane(d.x > 0 ? 1 : -1);
                }
                //up/down swipes currently unused
            }
            else if (touch.time - touch.startTime < tapMaxTime)
            {
                //Tap also currently unused for this so im leaving it empty for now

            }
        }
    }
}
