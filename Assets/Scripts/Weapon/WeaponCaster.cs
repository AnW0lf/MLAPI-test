using Game;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class WeaponCaster : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _hitDistance = 40f;
        [SerializeField] private float _rechargeDelay = 1f;

        private bool _canShoot = true;

        public void Shoot()
        {
            if (_canShoot == false) { return; }

            float angle = Vector3.Angle(new Vector3(0f, _camera.transform.forward.y, 0f), _camera.transform.forward);
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
                + _camera.transform.forward * 0.4f * (1f / Mathf.Cos(angle)));

            if (Physics.Raycast(ray, out RaycastHit hit, _hitDistance))
            {
                AddHit(hit);
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
            Transform parent = hit.transform;

            if (NetworkWeaponMessanger.Singleton != null)
            {
                if (parent.TryGetComponent(out Player.NetworkPlayer player))
                {
                    position = parent.InverseTransformPoint(position);
                    rotation *= parent.rotation;
                    NetworkWeaponMessanger.Singleton.HitPlayerServerRpc(player.OwnerClientId, position, rotation);
                }
                else if (parent.TryGetComponent(out Civilian civilian))
                {
                    position = parent.InverseTransformPoint(position);
                    rotation *= parent.rotation;
                    NetworkWeaponMessanger.Singleton.HitCivilianServerRpc(civilian.UniqueId, position, rotation);
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