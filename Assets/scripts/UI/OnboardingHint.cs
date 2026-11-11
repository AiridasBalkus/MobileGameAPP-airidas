using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Swipe / coin hint for the first 3 runs
public class OnboardingHint : MonoBehaviour
{
    const string GameScene = "Game";
    const string RunsKey = "hint_runs";
    const int RunsToShow = 3;
    const string SwipeText = "Swipe left / right to change lanes";
    const string CoinsText = "Collect coins to upgrade";
    const float SwipeHintMax = 8f;
    const float CoinsHintTime = 2.5f;
    const float FadeTime = 0.4f;

    CanvasGroup _group;
    TMP_Text _label;
    GameManager _game;
    bool _swiped;
    bool _counted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Hook()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        var active = SceneManager.GetActiveScene();
        if (active.name == GameScene) TryCreate(active);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameScene) TryCreate(scene);
    }

    static void TryCreate(Scene scene)
    {
        if (PlayerPrefs.GetInt(RunsKey, 0) >= RunsToShow) return;
        if (FindAnyObjectByType<OnboardingHint>()) return;

        Transform safeArea = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Canvas") continue;
            safeArea = root.transform.Find("SafeArea");
            if (safeArea) break;
        }
        if (!safeArea) { Debug.LogWarning("[Hint] Canvas/SafeArea not found"); return; }

        var go = new GameObject("OnboardingHint", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(safeArea, false);
        rt.anchorMin = new Vector2(0.08f, 0.60f);
        rt.anchorMax = new Vector2(0.92f, 0.68f);
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        // under the pause panel
        var pausePanel = safeArea.Find("PausePanel");
        if (pausePanel) rt.SetSiblingIndex(pausePanel.GetSiblingIndex());

        var bg = go.GetComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);
        bg.raycastTarget = false;

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        var trt = (RectTransform)textGo.transform;
        trt.SetParent(rt, false);
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(24f, 8f); trt.offsetMax = new Vector2(-24f, -8f);
        var text = textGo.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableAutoSizing = true;
        text.fontSizeMin = 28f;
        text.fontSizeMax = 64f;

        var hint = go.AddComponent<OnboardingHint>();
        hint._group = go.GetComponent<CanvasGroup>();
        hint._group.blocksRaycasts = false;
        hint._group.interactable = false;
        hint._label = text;
    }

    void Start()
    {
        _game = FindAnyObjectByType<GameManager>();
        if (_game) _game.StateChanged += OnStateChanged;
        LaneRunner.LaneChanged += OnLaneChanged;
        _ = RunAsync(destroyCancellationToken);
    }

    void OnDestroy()
    {
        if (_game) _game.StateChanged -= OnStateChanged;
        LaneRunner.LaneChanged -= OnLaneChanged;
    }

    void OnLaneChanged(int direction) => _swiped = true;

    void OnStateChanged(GameManager.State s)
    {
        if (_counted || (s != GameManager.State.Won && s != GameManager.State.Lost)) return;
        _counted = true;
        PlayerPrefs.SetInt(RunsKey, PlayerPrefs.GetInt(RunsKey, 0) + 1);
        PlayerPrefs.Save();
        if (_group) _group.alpha = 0f;
    }

    async Awaitable RunAsync(CancellationToken ct)
    {
        try
        {
            _label.SetText(SwipeText);
            _group.alpha = 1f;
            for (float t = 0f; !_swiped && t < SwipeHintMax && !_counted; t += Time.deltaTime)
                await Awaitable.NextFrameAsync(ct);

            if (!_counted)
            {
                _label.SetText(CoinsText);
                for (float t = 0f; t < CoinsHintTime && !_counted; t += Time.deltaTime)
                    await Awaitable.NextFrameAsync(ct);
            }

            for (float t = 0f; t < FadeTime && !_counted; t += Time.deltaTime)
            {
                _group.alpha = 1f - t / FadeTime;
                await Awaitable.NextFrameAsync(ct);
            }
            _group.alpha = 0f;
            gameObject.SetActive(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
    }
}
