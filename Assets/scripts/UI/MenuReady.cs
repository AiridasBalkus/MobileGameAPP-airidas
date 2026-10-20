using UnityEngine;

public class MenuReady : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"[perf] interactive {Time.realtimeSinceStartup:F2}s");
        enabled = false;   // first frame only
    }
}