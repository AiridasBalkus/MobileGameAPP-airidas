using UnityEngine;
using System;
using System.Threading;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform player; //players transform to spawn obstacles ahead of
    [SerializeField] ObstaclePool obstacles;
    [SerializeField] ObstaclePool pickups;
    public float spawnAheadDistanceZ = 40f; //distance ahead of player to spawn obstacles
    public float laneWidth = 2f; //width of each lane
    public float gapBetweenChunks = 14f;
    public float stopSpawningAfterZ = 1180f; //stop spawning obstacles after this z position
    [Header("Debugging - stress test")]
    public bool stressTest = false; //if true, will spawn obstacles every frame for testing purposes
    public float stressTestGap = 2f; //time interval between spawns during stress test

    const int Single = 0, PickupLine = 1, Wall = 2, Zigzag = 3, Corridor = 4;
    CancellationTokenSource _cts;
    float _nextChunkZ;


    void OnEnable()
    {
        _nextChunkZ = player.position.z + 20f; //empy space at the start
        _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
        _ = RunAsync(_cts.Token);
    }
    void OnDisable() { _cts.Cancel(); _cts.Dispose(); }
    async Awaitable RunAsync(CancellationToken ct)
    {
        try
        {
            await Awaitable.NextFrameAsync(ct);
            while (true)
            {
                // Fill the track up to spawnAhead in front of the player
                while (_nextChunkZ <= player.position.z + spawnAheadDistanceZ && _nextChunkZ < stopSpawningAfterZ)
                {
                    float length = SpawnChunk(PickType(_nextChunkZ), _nextChunkZ);
                    float gap = (stressTest && Debug.isDebugBuild) ? stressTestGap : gapBetweenChunks;
                    _nextChunkZ += length + gap;
                }
                await Awaitable.NextFrameAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { Debug.LogException(e); }
    }

    // Difficulty ramp: more chunk types unlock further down the lane
    static int PickType(float z)
    {
        int unlocked = z < 150f ? 2 : z < 400f ? 3 : 5;
        return UnityEngine.Random.Range(0, unlocked);
    }

    // Spawns one chunk starting at z; returns its length
    float SpawnChunk(int type, float z)
    {
        int flip = UnityEngine.Random.value < 0.5f ? -1 : 1;
        int lane = UnityEngine.Random.Range(-1, 2);

        switch (type)
        {
            case Single:
                Block(lane, z);
                return 0f;

            case PickupLine:                       // risk/reward: hazard next to the coins
                for (int i = 0; i < 5; i++) Coin(lane, z + i * 3f);
                Block(lane == 0 ? flip : 0, z + 6f);
                return 12f;

            case Wall:                             // one gap
                for (int l = -1; l <= 1; l++) if (l != lane) Block(l, z);
                return 0f;

            case Zigzag:
                Block(-flip, z); Block(0, z + 8f); Block(flip, z + 16f);
                return 16f;

            default:                               // Corridor: middle lane only
                for (int i = 0; i <= 2; i++) { Block(-1, z + i * 5f); Block(1, z + i * 5f); }
                return 10f;
        }
    }

    void Block(int lane, float z) => obstacles.Spawn(new Vector3(lane * laneWidth, 1f, z));
    void Coin(int lane, float z) => pickups.Spawn(new Vector3(lane * laneWidth, 1f, z));
}
