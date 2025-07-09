using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet attributes")]
    [SerializeField] float bulletLifeTime = 1f;
    [SerializeField] float bulletShotSpeed = 2f;

    private float _lifeTimeCountDown;
    Rigidbody2D bulletRb2D;
    CircleCollider2D bulletCollider2D;

    private Vector3 spawnPos;
    private Shoot playerGun;

    void Start()
    {
        bulletRb2D = this.GetComponent<Rigidbody2D>();
        _lifeTimeCountDown = bulletLifeTime;
        bulletCollider2D = this.GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        SetBulletParent();
        _lifeTimeCountDown = bulletLifeTime;
        transform.position = playerGun.GetGunPos();
        spawnPos = transform.position;
    }

    private void OnDisable()
    {
        transform.position = spawnPos;
        playerGun.AddBulletToTheMagazine(this.gameObject.GetComponent<Bullet>());
    }

    //Update is called once per frame
    void Update()
    {
        DisableBullet();
    }

    private void FixedUpdate()
    {
        if (this.gameObject.activeSelf) { MoveBullet(); }
    }

    private void MoveBullet()
    {
        if (bulletRb2D == null) { return; }

        bulletRb2D.AddForceX(bulletShotSpeed, ForceMode2D.Impulse);
    }

    private bool IsLifeTimeOver()
    {
        _lifeTimeCountDown -= Time.deltaTime;
        if (_lifeTimeCountDown <= 0)
        {
            _lifeTimeCountDown = 0f;
            return true;
        }
        return false;
    }

    private bool HasBullletCollided()
    {
        Collider2D bulletoverlap = Physics2D.OverlapCircle(this.transform.position, bulletCollider2D.radius * transform.lossyScale.x, LayerMask.GetMask("ground"));

        return bulletoverlap != null;
    }

    private void DisableBullet()
    {
        if (!this.gameObject.activeSelf) { return; }
        if (IsLifeTimeOver() || HasBullletCollided())
        {

            this.gameObject.SetActive(false);
        }
    }

    private void SetBulletParent()
    {
        if(this.gameObject.GetComponentInParent<Shoot>() != null)
        {
            playerGun = this.gameObject.GetComponentInParent<Shoot>();
        }
    }
}
