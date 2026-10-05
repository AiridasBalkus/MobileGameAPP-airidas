using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float despawnBehind = 10f;
    ObstaclePool _pool;
    Transform _player;
    public Obstacle Init(ObstaclePool pool, Transform player)
    {
        _pool = pool; _player = player; return this;
    }
    void OnEnable() { }
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true; //make sure the collider is a trigger
    }
    private void OnTriggerEnter(Collider other)
    {
        var hit= other.GetComponent<PlayerCollision>();
        if (hit != null)
        {
            hit.OnHitObstacle();
        }
    }
    void Update()
    {
        if(transform.position.z < _player.position.z - despawnBehind)
            _pool.Release(this);
    }

}
