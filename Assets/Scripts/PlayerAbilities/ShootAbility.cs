using System;
using UnityEngine;

public class ShootAbility : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootingForce = 200f;

    private ObjectPool bulletPool;

    private void Awake()
    {
        bulletPool = FindAnyObjectByType<ObjectPool>();
    }

    public void Shoot()
    {
        Rigidbody clonedProjectile = bulletPool.RetrieveAvailableBullet().GetRigidbody();

        if (clonedProjectile == null)
        {
            Debug.LogError("No projectile available in the pool!");
            return;
        }
        
        clonedProjectile.position = firePoint.position;
        clonedProjectile.rotation = firePoint.rotation;
        clonedProjectile.GetComponentInChildren<TrailRenderer>().emitting = true;
        clonedProjectile.GetComponentInChildren<TrailRenderer>().Clear();
        clonedProjectile.AddForce(firePoint.forward * shootingForce);
    }



    // [SerializeField] private Transform _firePoint;
    // public Transform firePoint => _firePoint;


    // [SerializeField] private float _shootingForce;
    // public float GetShootForce() => _shootingForce;


    // [Header("Object Pools")]
    // [SerializeField] private ObjectPool _bulletPool;
    // [SerializeField] private ObjectPool _rocketPool;
    // public ObjectPool bulletPool => _bulletPool;
    // public ObjectPool rocketPool => _rocketPool;

    // [SerializeField] private GameObject _weaponSignifier;

    // private IShootStrategy _currentShootingStrategy;

    // public Action<int> OnChangeStrategy;

    // private void Start()
    // {
    //     _currentShootingStrategy = new BulletShootStrategy(_weaponSignifier);
    //     OnChangeStrategy?.Invoke(0);
    // }


    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         _currentShootingStrategy = new BulletShootStrategy(_weaponSignifier);
    //         OnChangeStrategy?.Invoke(0);

    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         _currentShootingStrategy = new RocketShootStrategy(_weaponSignifier);
    //         OnChangeStrategy?.Invoke(1);
    //     }
    // }


    // public void Shoot()
    // {
    //     _currentShootingStrategy?.Shoot(this);
    // }
}
