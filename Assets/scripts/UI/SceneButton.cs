using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public void Load(string sceneName)
    {
        if (SceneLoader.Instance) _ = SceneLoader.Instance.Load(sceneName, Application.exitCancellationToken);
        else SceneManager.LoadScene(sceneName);
    }

}
