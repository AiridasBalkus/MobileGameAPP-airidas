using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour
{
    [SerializeField] GameManager game;
    [SerializeField] TMP_Text label;
    int _shown = -1;

    void Update()
    {
        int score = game.Score;
        if (score == _shown) return;
        _shown = score;
        label.SetText("{0}", score);
    }
}
