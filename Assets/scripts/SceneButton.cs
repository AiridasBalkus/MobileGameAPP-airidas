using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public void Load(string sceneName) => SceneManager.LoadScene(sceneName);

}
