using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Placer : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.one;

        public void Place(Transform obj)
        {
            SetRandomPosition(obj);
            SetRandomRotation(obj);
        }

        public void SetRandomPosition(Transform obj)
        {
            Vector3? point;
            do
            {
                Vector2 castPosition = new Vector2(
                    transform.position.x + Random.Range(-_size.x / 2f, _size.x / 2f),
                    transform.position.z + Random.Range(-_size.z / 2f, _size.z / 2f)
                    );
                point = Cast(castPosition);
            } while (point == null);
            obj.position = (Vector3)point;
        }

        public void SetRandomRotation(Transform obj)
        {
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            obj.rotation = rotation;
        }

        private Vector3? Cast(Vector2 position)
        {
            Vector3? result = null;
            Vector3 origin = new Vector3(position.x, transform.position.y + _size.y, position.y);
            Ray ray = new Ray(origin, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, _size.y))
            {
                result = hit.point;
            }
            return result;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 center = transform.position + transform.up * _size.y * 0.5f;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, _size);
        }
    }
}