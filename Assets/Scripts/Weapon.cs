using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletForce;
    [SerializeField] private float _shootDelay;
    private float _shootTimer;
    private void Update()
    {
        if (_shootTimer > 0)
        {
            _shootTimer -= Time.deltaTime;
        }

       // Shoot();
    }

    public void Shoot()
    {
        if (_shootTimer > 0)
        {
            return;
        }

        GameObject newBullet = Instantiate(_bulletPrefab, _bulletSpawnTransform.position, _bulletSpawnTransform.rotation);
        newBullet.GetComponent<Rigidbody>().AddForce(_bulletSpawnTransform.forward * _bulletForce, ForceMode.Impulse);
        _shootTimer = _shootDelay;
    }
}
