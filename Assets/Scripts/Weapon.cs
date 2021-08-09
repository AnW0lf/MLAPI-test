using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletForce;
    [SerializeField] private float _verticalForceMult;
    [SerializeField] private float _shootDelay;
    [SerializeField] private GameObject _trajectoryLinePrefab;
    //private LineRenderer _trajectoryLineRenderer;

    private bool _ready = true;
    //private float _bulletMass;
    [SerializeField] private int _positionsCount;
    //private List<Vector3> _linePoints = new List<Vector3>();

    private Transform _mainTransform;

    private void Start()
    {
        //_trajectoryLineRenderer = Instantiate(_trajectoryLinePrefab, transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        //_bulletMass = _bulletPrefab.GetComponent<Rigidbody>().mass;
        //_trajectoryLineRenderer.transform.SetParent(null);
        _mainTransform = GetComponentInParent<Player.PlayerController>().transform;     
    }

    //private void Update()
    //{
    //    UpdateTrajectory();
    //}

    public void Shoot()
    {
        if(_ready == false) { return; }

        GameObject newBullet = Instantiate(_bulletPrefab, _bulletSpawnTransform.position, _bulletSpawnTransform.rotation);
        newBullet.GetComponent<Rigidbody>().AddForce((_bulletSpawnTransform.forward + _mainTransform.up * _verticalForceMult) * _bulletForce);
        _ready = false;
        StartCoroutine(ShootDelay(_shootDelay));     
    }

    private IEnumerator ShootDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _ready = true;
    }

    //public void UpdateTrajectory()
    //{
    //    _trajectoryLineRenderer.transform.position = transform.position;

    //    // Vector3 velocity = ((((_bulletSpawnTransform.position - transform.position).normalized + transform.up) * _bulletForce) / _bulletMass) * Time.fixedDeltaTime;
    //    Vector3 velocity = (((_bulletSpawnTransform.forward + _mainTransform.up * _verticalForceMult) * _bulletForce) / _bulletMass) * Time.fixedDeltaTime;
    //    float flightDuration = (2 * velocity.y) / Physics.gravity.y;
    //    float stepTime = flightDuration / _positionsCount;

    //    _linePoints.Clear();

    //    for (int i = 0; i < _positionsCount; i++)
    //    {
    //        float stepTimePassed = stepTime * i;

    //        Vector3 shiftVector = new Vector3(velocity.x * stepTimePassed,
    //                                          velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
    //                                          velocity.z * stepTimePassed);

    //        _linePoints.Add(-shiftVector);
    //    }

    //    _trajectoryLineRenderer.positionCount = _linePoints.Count;
    //    _trajectoryLineRenderer.SetPositions(_linePoints.ToArray());
    //}
}
