using Assets.Scripts.Network;
using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class WeaponCaster : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _rechargeDelay = 1f;
        [SerializeField] private GameObject _gun = null;

        private bool _canShoot = true;

        public void Shoot()
        {
            if (_gun.activeSelf == false) { return; }
            if (_canShoot == false) { return; }
            if (_camera == null) { return; }
            
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            RaycastHit[] hits = Physics.RaycastAll(ray, _camera.farClipPlane);
            if (hits != null && hits.Length > 0)
            {
                var sortedHits = hits.ToList();
                sortedHits.Sort((hit_1, hit_2) =>
                    Vector3.Distance(transform.position, hit_1.point)
                    .CompareTo(Vector3.Distance(transform.position, hit_2.point))
                    );

                RaycastHit hit = sortedHits.First((h) => h.transform != transform);

                if (hit.transform != null)
                {
                    AddHit(hit);
                }

                StartCoroutine(Recharge());
            }
        }

        private IEnumerator Recharge()
        {
            _canShoot = false;
            yield return new WaitForSeconds(_rechargeDelay);
            _canShoot = true;
        }

        private void AddHit(RaycastHit hit)
        {
            Vector3 position = hit.point + hit.normal * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(-hit.normal, Vector3.forward);
            Transform target = hit.transform;

            if (NetworkWeaponMessanger.Singleton != null)
            {
                ulong id;
                if (target.TryGetComponent(out Remote renote))
                {
                    id = renote.Architector.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitActorServerRpc(id, position, rotation);
                }
                else if (target.TryGetComponent(out Local local))
                {
                    id = local.Architector.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitActorServerRpc(id, position, rotation);
                }
                else
                {
                    NetworkWeaponMessanger.Singleton.HitObjectServerRpc(position, rotation);
                }
            }
            else
            {
                Debug.LogError($"NetworkWeaponMessanger object not found!..");
            }
        }
    }
}