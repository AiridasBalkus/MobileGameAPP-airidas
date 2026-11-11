using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour
{
    [SerializeField] GameManager game;
    [SerializeField] TMP_Text label;
    // KI-02: the parent SafeArea already keeps us inside Screen.safeArea, but with gesture /
    // swipe-in navigation Android reports no bottom inset, so the bar covered the score.
    [SerializeField] float bottomMarginDp = 48f;
    int _shown = -1;

    void Start()
    {
        // Start, not Awake: CanvasScaler has set scaleFactor by now.
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
