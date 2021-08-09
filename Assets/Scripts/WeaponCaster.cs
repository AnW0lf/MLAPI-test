using UnityEngine;

namespace Assets.Scripts
{
    public class WeaponCaster : MonoBehaviour
    {
        [SerializeField] private GameObject _hitPrefab = null;
        [SerializeField] private Collider _ignoreCollider = null;
        [SerializeField] private Camera _camera = null;

        public void Shoot()
        {
            float angle = Vector3.Angle(new Vector3(0f, _camera.transform.forward.y, 0f), _camera.transform.forward);
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
                + _camera.transform.forward * 0.4f * (1f / Mathf.Cos(angle)));
            if (Physics.Raycast(ray, out RaycastHit hit, 40f))
            {
                if (hit.transform.CompareTag("Civilian"))
                {
                    
                }
                else if (hit.transform.CompareTag("Player"))
                {

                }
                else
                {

                }
                
                AddHit(hit);
            }
        }

        private void AddHit(RaycastHit hit)
        {
            Vector3 position = hit.point + hit.normal * 0.2f;
            Quaternion rotation = Quaternion.LookRotation(-hit.normal, Vector3.forward);
            Transform parent = hit.transform;

            Transform hitTransform = Instantiate(_hitPrefab, parent).transform;
            hitTransform.position = position;
            hitTransform.rotation = rotation;
        }

    }
}