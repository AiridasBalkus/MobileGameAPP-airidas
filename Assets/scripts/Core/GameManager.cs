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

    const int LevelId = 1;
    static int s_attempt;
    float _startTime;
    int _coinsEarned;

    [SerializeField] LaneRunner runner;
    [SerializeField] float resultDelay = 1f; // seconds to wait before going to result scene
    [SerializeField] float finishDistance = 1200f;
    [SerializeField] int pickupValue = 10;
    [SerializeField] int coinsPerPickup = 5;
    int _bonus;
    public int Distance => Mathf.Max(0, Mathf.FloorToInt(runner.transform.position.z - _startZ));
    public int Score => Distance + _bonus;

    public static bool LastRunWon { get; private set; }
    float _startZ;

    void OnEnable() => LifecycleGuard.PausedChanged += OnPausedChanged;
    void OnDisable() => LifecycleGuard.PausedChanged -= OnPausedChanged;
    void Start()
    {
        _startZ = runner.transform.position.z;
        SetState(State.Playing);
        _startTime = Time.time;
        s_attempt++;
        Telemetry.Log("level_start", ("level_id", LevelId), ("attempt", s_attempt),
            ("coins", Wallet.Instance ? Wallet.Instance.Coins : 0));
    }
    void Update()
    {
        if (Current == State.Playing && Distance >= finishDistance) Win();
    }
    public void AddPickup()
    {
        if (Current != State.Playing) return;
        _bonus += pickupValue;
        if (Wallet.Instance)
        {
            Wallet.Instance.Earn(coinsPerPickup); _coinsEarned += coinsPerPickup;
        }

    }
    public void Win() => EndRun(State.Won);
    public void Lose() => EndRun(State.Lost);
    void OnPausedChanged(bool paused)
    {
        if (Current == State.Won || Current == State.Lost) return;
        SetState(paused ? State.Paused : State.Playing);
    }
    void EndRun(State end)
    {
        if (Current == State.Won || Current == State.Lost) return;
        SetState(end);
        float t = Mathf.Round((Time.time - _startTime) * 10f) / 10f;
        if (end == State.Won)
            Telemetry.Log("level_complete", ("level_id", LevelId), ("time_s", t),
                ("coins_earned", _coinsEarned), ("score", Score));
        else
            Telemetry.Log("level_fail", ("level_id", LevelId), ("time_s", t),
                ("cause", "obstacle"), ("distance", Distance), ("coins_earned", _coinsEarned));
        runner.enabled = false;
        LastRunWon = end == State.Won;
        LastScore = Score;
        if (LastScore > HighScore) { PlayerPrefs.SetInt("highscore", LastScore); PlayerPrefs.Save(); }
        _ = GoToResultAsync(destroyCancellationToken);
    }
    async Awaitable GoToResultAsync(CancellationToken ct)
    {
        try
        {
            await Awaitable.WaitForSecondsAsync(resultDelay, ct);
            _ = SceneLoader.Instance.Load("Result", Application.exitCancellationToken);
        }
        catch (OperationCanceledException) { Debug.Log("Result scene load canceled."); }
    }
    void SetState(State s) { Current = s; StateChanged?.Invoke(s); }
}
