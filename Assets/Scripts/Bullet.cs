using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _thisRigidbody;
    [SerializeField] private float _lifetime;

    private void Update()
    {
        if (_lifetime > 0)
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {       
        _thisRigidbody.velocity = Vector3.zero;
        _thisRigidbody.isKinematic = true;
        transform.SetParent(collision.transform);

        if (collision.transform.tag == "Civilian")
        {
            _lifetime = -1;
        }
    }
}
