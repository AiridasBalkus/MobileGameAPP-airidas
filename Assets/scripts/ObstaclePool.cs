using UnityEngine;
using System.Collections.Generic;

public class ObstaclePool : MonoBehaviour
{
    [SerializeField] Obstacle prefab;
    [SerializeField] Transform player;
    [SerializeField] int size = 24;
    Stack<Obstacle> _free;

    void Awake()
    {
        _free = new Stack<Obstacle>(size);
        for (int i = 0; i < size; i++) _free.Push(Create());
    }

    Obstacle Create()
    {
        var obstacle = Instantiate(prefab, transform);   // children of the pool
        obstacle.gameObject.SetActive(false);
        return obstacle.Init(this, player);
    }

    public Obstacle Spawn(Vector3 position)
    {
        var obstacleSpawn = _free.Count > 0 ? _free.Pop() : Create();
        obstacleSpawn.transform.SetPositionAndRotation(position, Quaternion.identity);
        obstacleSpawn.gameObject.SetActive(true);
        return obstacleSpawn;
    }

    public void Release(Obstacle obstacle)
    {
        if (!obstacle.gameObject.activeSelf) return;
        obstacle.gameObject.SetActive(false);
        _free.Push(obstacle);
    }
}