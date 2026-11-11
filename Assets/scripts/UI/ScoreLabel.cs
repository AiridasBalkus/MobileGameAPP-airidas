using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour
{
    [SerializeField] GameManager game;
    [SerializeField] TMP_Text label;
    // lift above the nav bar (KI-02)
    [SerializeField] float bottomMarginDp = 48f;
    int _shown = -1;

    void Start()
    {
        var canvas = label.canvas ? label.canvas.rootCanvas : null;
        float scale = canvas && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
        float px = bottomMarginDp * Mathf.Max(Screen.dpi, 160f) / 160f;
        var rt = label.rectTransform;
        rt.anchoredPosition += new Vector2(0f, px / scale);
    }

    void Update()
    {
        int score = game.Score;
        if (score == _shown) return;
        _shown = score;
        label.SetText("{0}", score);
    }
}
