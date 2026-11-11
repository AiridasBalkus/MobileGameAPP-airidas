using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class tapSwipeInput : MonoBehaviour
{

    public float swipeDp = 50f; // Minimum swipe distance in dp
    public float tapMaxTime = 0.3f; // Maximum time for a tap in seconds

    public LaneRunner runner;

    //This is reused for UI hit tests so a swipe that starts on a HUD button is not read as lane input.
    static readonly List<RaycastResult> s_hits = new List<RaycastResult>(8);
    PointerEventData _pointer;
    EventSystem _pointerOwner;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        if(LifecycleGuard.IsPaused) return; // Don't process input if the game is paused
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase != TouchPhase.Ended) continue;
            if (StartedOnButton(touch.startScreenPosition)) continue;
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

    // True if the touch began on an interactable button/toggle (Pause, Upgrade...).
    bool StartedOnButton(Vector2 screenPos)
    {
        var es = EventSystem.current;
        if (es == null) return false;
        if (_pointer == null || _pointerOwner != es) { _pointer = new PointerEventData(es); _pointerOwner = es; }
        _pointer.position = screenPos;
        s_hits.Clear();
        es.RaycastAll(_pointer, s_hits);
        for (int i = 0; i < s_hits.Count; i++)
        {
            var sel = s_hits[i].gameObject.GetComponentInParent<Selectable>();
            if (sel != null && sel.IsInteractable()) return true;
        }
        return false;
    }
}
