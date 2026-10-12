using UnityEngine;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text bestText;
    [SerializeField] TMP_Text titleText;
    void Start()
    {
        scoreText.SetText("Score {0}", GameManager.LastScore);
        bestText.SetText("Best {0}", GameManager.HighScore);
        titleText.SetText(GameManager.LastRunWon ? "Finished!" : "Crashed");
    }
}
