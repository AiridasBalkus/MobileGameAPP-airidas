using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State { Playing, Paused, Won, Lost }
    public State Current { get; private set; } = State.Playing;
    public event Action<State> StateChanged;
    public static int LastScore { get; private set; }
    public static int HighScore => PlayerPrefs.GetInt("highscore", 0);

    [SerializeField] LaneRunner runner;
    [SerializeField] float resultDelay = 1f; // seconds to wait before going to result scene
    float _startZ;
    public int Score => Mathf.Max(0, Mathf.FloorToInt(runner.transform.position.z - _startZ));

    void OnEnable() => LifecycleGuard.PausedChanged += OnPausedChanged;
    void OnDisable() => LifecycleGuard.PausedChanged -= OnPausedChanged;

    void Start()
    {
        _startZ = runner.transform.position.z;
        SetState(State.Playing);
    }

    void OnPausedChanged(bool paused)
    {
        if (Current == State.Won || Current == State.Lost) return;
        SetState(paused ? State.Paused : State.Playing);
    }

    public void Lose()
    {
        if (Current == State.Lost) return;
        SetState(State.Lost);
        runner.enabled = false;
        LastScore = Score;
        if (LastScore > HighScore) { PlayerPrefs.SetInt("highscore", LastScore); PlayerPrefs.Save(); }
        _ = GoToResultAsync(destroyCancellationToken);
    }

    async Awaitable GoToResultAsync(CancellationToken ct)
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(resultDelay, ct);
            await SceneManager.LoadSceneAsync("Result");
        }
        catch (OperationCanceledException) { Debug.Log("Result scene load canceled."); }
    }
    void SetState(State s) { Current = s; StateChanged?.Invoke(s); }
}
