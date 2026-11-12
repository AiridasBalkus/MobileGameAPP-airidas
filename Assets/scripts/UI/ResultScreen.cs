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
        if (GameManager.LastRunNewBest)
        {
            bestText.SetText("New best! {0}", GameManager.HighScore);
            bestText.color = new Color(1f, 0.85f, 0.2f);
        }
        else bestText.SetText("Best {0}", GameManager.HighScore);
        titleText.SetText(GameManager.LastRunWon ? "Finished!" : "Crashed");
        var audio = AudioService.Instance;
        if (GameManager.LastRunNewBest && audio) audio.NewBest();
    }
}
