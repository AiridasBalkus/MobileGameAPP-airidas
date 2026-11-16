using UnityEngine;

public class StoreShot : MonoBehaviour
{
    [SerializeField] private GameObject hudRoot;   // HUD to hide
    [SerializeField] private int superSize = 1;    // 2 = double res

    public async Awaitable CaptureAsync(string tag)
    {
        hudRoot.SetActive(false);
        await Awaitable.NextFrameAsync();          // HUD is gone now
        string file = $"{tag}_{Screen.width}x{Screen.height}.png";
        // Android: Unity prepends Application.persistentDataPath
        ScreenCapture.CaptureScreenshot(file, superSize);
        await Awaitable.WaitForSecondsAsync(0.5f); // file write
        hudRoot.SetActive(true);
        Debug.Log($"Saved {Application.persistentDataPath}/{file}");
    }
}
