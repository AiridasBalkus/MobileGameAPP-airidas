using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform player; //players transform to spawn obstacles ahead of
    public GameObject obstaclePrefab; //prefab to spawn
    public float spawnAheadDistanceZ = 30f; //distance ahead of player to spawn obstacles
    public float spawnInterval = 1.5f; //time interval between spawns
    public float laneWidth = 2f; //width of each lane
    public int laneHalfCount = 1; //matches LaneRunner's lane count/2
    float _timer;

    [Header("Debugging - stress test")]
    public bool stressTest = false; //if true, will spawn obstacles every frame for testing purposes
    public float stressTestSpawnInterval = 0.05f; //time interval between spawns during stress test

    void Update()
    {
        if (player == null || obstaclePrefab == null) return;
        _timer += Time.deltaTime;
        float interval = (stressTest && Debug.isDebugBuild) ? stressTestSpawnInterval : spawnInterval;
        if (_timer < interval) return;
        _timer = 0f;

        int lane = Random.Range(-laneHalfCount, laneHalfCount + 1); //random lane selection
        Vector3 spawnPosition = new Vector3(lane * laneWidth, 1f, player.position.z + spawnAheadDistanceZ); //spawn position based on lane and distance ahead of player
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);//spawns the obstacle right now its a cube - AIRIDAS REMEBER TO CHANGE IT
    }
}
