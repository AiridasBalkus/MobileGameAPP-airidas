using UnityEngine;
using System;
using System.Threading;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform player; //players transform to spawn obstacles ahead of
    [SerializeField] ObstaclePool pool;
    public float spawnAheadDistanceZ = 30f; //distance ahead of player to spawn obstacles
    public float spawnInterval = 1.5f; //time interval between spawns
    public float laneWidth = 2f; //width of each lane
    public int laneHalfCount = 1; //matches LaneRunner's lane count/2
    float _timer;

    [Header("Debugging - stress test")]
    public bool stressTest = false; //if true, will spawn obstacles every frame for testing purposes
    public float stressTestSpawnInterval = 0.05f; //time interval between spawns during stress test

    CancellationTokenSource _cts;

    void OnEnable()
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
        _ = RunAsync(_cts.Token);   // this was a timer in Update()
    }
    void OnDisable() { _cts.Cancel(); _cts.Dispose(); }
    async Awaitable RunAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                float interval = (stressTest && Debug.isDebugBuild) ? stressTestSpawnInterval : spawnInterval;
                await Awaitable.WaitForSecondsAsync(interval, ct);

                int lane = UnityEngine.Random.Range(-laneHalfCount, laneHalfCount + 1);
                pool.Spawn(new Vector3(lane * laneWidth, 1f, player.position.z + spawnAheadDistanceZ));
            }
        }
        catch (OperationCanceledException) { Debug.Log("Obstacle spawning cancelled."); }
    }
}
