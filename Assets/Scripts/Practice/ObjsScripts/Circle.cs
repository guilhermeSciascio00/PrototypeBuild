using UnityEngine;

public class Circle : MonoBehaviour, IFallable, IRespawnable
{
    [SerializeField] float circleFallForce;
    [SerializeField] float spawnTimer;
    private float _spawnTimerCountDown;
    private bool _isRespawning = false;

    private Vector2 _startPosition;
    private Rigidbody2D rb2d;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 1f;
        _startPosition = transform.position;
    }

    void Update()
    {
        RespawnObject();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Fall();
    }

    private void RespawnObject()
    {
        if (!_isRespawning) return;

        if (IsRespawnTimeOver())
        {
            transform.position = _startPosition;
            _isRespawning = false;
        }
        
    }

    private bool IsRespawnTimeOver()
    {
        _spawnTimerCountDown -= Time.deltaTime;
        if(_spawnTimerCountDown <= 0)
        {
            _spawnTimerCountDown = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Fall()
    {
        rb2d.AddForceY(-circleFallForce);
    }

    public void Respawn()
    {
        _isRespawning = true;
        _spawnTimerCountDown = spawnTimer;
    }

}
