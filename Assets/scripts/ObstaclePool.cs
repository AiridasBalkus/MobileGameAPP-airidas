using UnityEngine;
using System.Collections.Generic;

public class ObstaclePool : MonoBehaviour
{
    [SerializeField] PooledItem prefab;
    [SerializeField] Transform player;
    [SerializeField] int size = 24;
    Stack<PooledItem> _free;

    void Awake()
    {
        _free = new Stack<PooledItem>(size);
        for (int i = 0; i < size; i++) _free.Push(Create());
    }

    PooledItem Create()
    {
        var obstacle = Instantiate(prefab, transform);   // children of the pool
        obstacle.gameObject.SetActive(false);
        return obstacle.Init(this, player);
    }

    public PooledItem Spawn(Vector3 position)
    {
        var item = _free.Count > 0 ? _free.Pop() : Create();
        item.transform.SetPositionAndRotation(position, Quaternion.identity);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(PooledItem item)
    {
        if (!item.gameObject.activeSelf) return;
        item.gameObject.SetActive(false);
        _free.Push(item);
    }
}