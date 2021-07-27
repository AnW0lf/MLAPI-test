using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletForce;
    [SerializeField] private float _shootDelay;

    private bool _ready = true;

    public void Shoot()
    {
        if(_ready == false) { return; }

        GameObject newBullet = Instantiate(_bulletPrefab, _bulletSpawnTransform.position, _bulletSpawnTransform.rotation);
        newBullet.GetComponent<Rigidbody>().AddForce(_bulletSpawnTransform.forward * _bulletForce, ForceMode.Impulse);
        newBullet.GetComponent<NetworkObject>().Spawn();
        _ready = false;
        StartCoroutine(ShootDelay(_shootDelay));
    }

    private IEnumerator ShootDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _ready = true;
    }
}
