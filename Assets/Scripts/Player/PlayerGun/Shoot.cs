using UnityEngine;
using System.Collections.Generic;

public class Shoot : MonoBehaviour
{
    [SerializeField] InputManager _inputSystem;

    [SerializeField] private List<Bullet> bullets;
    [SerializeField] private Queue<Bullet> spawnedBullets = new Queue<Bullet>();

    [SerializeField] private Vector3 shootOffset;

    private void Awake()
    {
        CreateBullets();
    }

    private void Update()
    {
        ShootBullet();
    }

    private void CreateBullets()
    {
        for(int i = 0; i < bullets.Count; i++)
        {
            spawnedBullets.Enqueue(CreateOneBullet(i));
        }
    }

    private Bullet CreateOneBullet(int num)
    {
        if (bullets[num] == null)  return null; 

       return Instantiate(bullets[num], transform.position + shootOffset, Quaternion.identity, this.gameObject.transform);
    }


    private void ShootBullet()
    {
        if (_inputSystem.WasAttackButtonPressed)
        {
            if (spawnedBullets.Count == 0)
            {
                Debug.LogWarning("There aren't any more bullets to shoot");
                return;
            }

            Bullet currentBullet = spawnedBullets.Dequeue();
            currentBullet.gameObject.SetActive(true);
        }
    }

    public void AddBulletToTheMagazine(Bullet bulletToAdd)
    {
        spawnedBullets.Enqueue(bulletToAdd);
    }

    public Vector2 GetGunPos() => this.transform.position + shootOffset;
}
