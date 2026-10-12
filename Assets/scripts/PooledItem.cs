using UnityEngine;

public abstract class PooledItem : MonoBehaviour
{
    [SerializeField] float despawnBehind = 10f;
    protected ObstaclePool Pool { get; private set; }
    Transform _player;
    public PooledItem Init(ObstaclePool pool, Transform player)
    {
        Pool = pool; _player = player; return this;
    }
    protected virtual void Update()
    {
        if (transform.position.z < _player.position.z - despawnBehind) Pool.Release(this);
    }
}
