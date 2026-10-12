using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Slider bar;
    [SerializeField] GameObject loadingScreen;
    public static SceneLoader Instance { get; private set; }
    public static bool IsLoading { get; private set; }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        loadingScreen.SetActive(false);
    }
    public async Awaitable Load(string scene, CancellationToken ct)
    {
        Debug.Log($"[Loader] Load({scene}) called, IsLoading={IsLoading}");
        if (IsLoading) return;
        IsLoading = true;
        var stopwatch = Stopwatch.StartNew();
        bar.value = 0f;
        loadingScreen.SetActive(true);
        try
        {
            var op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                bar.value = op.progress / 0.9f;
                await Awaitable.NextFrameAsync(ct);
            }
            bar.value = 1f;
            op.allowSceneActivation = true;
            await op;
            Debug.Log($"[perf] scene {scene} loaded in {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (OperationCanceledException) { }
        finally
        {
            loadingScreen.SetActive(false);//so player dont get stuck on loading screen if scene load is canceled
            IsLoading = false;
        }
    }
}