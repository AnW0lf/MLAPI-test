using MLAPI.Messaging;
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
        if (collision.transform.CompareTag("Civilian"))
        {
            
        }
        else if (collision.transform.CompareTag("Player"))
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HitCivilian(ulong civilianId, Vector3 position)
    {

    }

    private void HitPlayer(ulong playerId, Vector3 position)
    {

    }
}
