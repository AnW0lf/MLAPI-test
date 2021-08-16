using Game;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class WeaponCaster : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _hitDistance = 40f;
        [SerializeField] private float _rechargeDelay = 1f;
        [SerializeField] private GameObject _gun = null;

        private bool _canShoot = true;

        public void Shoot()
        {
            if (_canShoot == false) { return; }

            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            RaycastHit[] hits = Physics.RaycastAll(ray, _hitDistance);
            if (hits != null && hits.Length > 0)
            {
                RaycastHit hit = hits.First((h) => h.transform != transform);

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

        private bool _holded = false;
        private void Update()
        {
            if (_gun.activeSelf)
            {
                if (Input.GetAxis("Fire1") > 0f)
                {
                    Shoot();
                }
            }

            if (Input.GetAxis("Fire2") > 0f)
            {
                if (_holded == false)
                {
                    _gun.SetActive(!_gun.activeSelf);
                }
                if (_holded == false)
                {
                    _holded = true;
                }
            }
            else if (_holded)
            {
                _holded = false;
            }
        }
    }
}