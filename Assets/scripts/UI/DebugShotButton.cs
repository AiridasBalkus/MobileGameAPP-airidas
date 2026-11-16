using UnityEngine;
using UnityEngine.UI;

public class DebugShotButton : MonoBehaviour
{
    [SerializeField] private StoreShot shot;
    [SerializeField] private Button button;
    private int _n;

    private void Awake()
    {
        gameObject.SetActive(Debug.isDebugBuild);
        button.onClick.AddListener(() => _ = shot.CaptureAsync($"shot{_n++:00}"));
    }
}
