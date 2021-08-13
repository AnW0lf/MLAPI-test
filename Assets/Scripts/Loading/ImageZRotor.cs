using UnityEngine;

namespace Assets.Scripts.Loading
{
    public class ImageZRotor : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;

        void Update()
        {
            transform.Rotate(Vector3.forward, _speed * Time.deltaTime);
        }
    }
}