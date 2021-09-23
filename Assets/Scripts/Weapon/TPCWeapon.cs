using Assets.Scripts.Network;
using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class TPCWeapon : MonoBehaviour
    {
        [SerializeField] private Transform _weapon = null;
        [SerializeField] private float _rechargeDelay = 1f;
        [SerializeField] private float _shootDistance = 50f;

        private bool _canShoot = true;

        private void OnDrawGizmos()
        {
            Vector3 from = _weapon.position;
            Vector3 to = from + _weapon.forward * 10f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(from, to);
        }

        public void Shoot(Transform exclude = null)
        {
            if (_canShoot == false) { return; }

            Ray ray = new Ray(_weapon.position + _weapon.forward * 0.5f, _weapon.forward);

            RaycastHit[] hits = Physics.RaycastAll(ray, _shootDistance);
            if (hits != null && hits.Length > 0)
            {
                var sortedHits = hits
                    .Where((sh) => sh.transform != exclude
                    && sh.collider.isTrigger == false)
                    .ToList();

                if (sortedHits.Count == 0) { return; }

                sortedHits.Sort((hit_1, hit_2) =>
                    Vector3.Distance(transform.position, hit_1.point)
                    .CompareTo(Vector3.Distance(transform.position, hit_2.point))
                    );

                RaycastHit hit = sortedHits[0];

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
            if (hit.transform == null) { return; }

            Vector3 position = hit.point + hit.normal * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(-hit.normal, Vector3.forward);
            Transform target = hit.transform;

            if (NetworkWeaponMessanger.Singleton != null)
            {
                ulong id;
                if (target.TryGetComponent(out Remote remote))
                {
                    id = remote.Architector.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitPlayerServerRpc(id, position, rotation);
                }
                else if (target.TryGetComponent(out Local local))
                {
                    id = local.Architector.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitPlayerServerRpc(id, position, rotation);
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