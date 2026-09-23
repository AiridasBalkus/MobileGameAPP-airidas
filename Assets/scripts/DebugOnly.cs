using UnityEngine;

// Hides this GameObject in release builds. Put it on debug-only UI.
public class DebugOnly : MonoBehaviour
{
    void Awake() => gameObject.SetActive(Debug.isDebugBuild);
}