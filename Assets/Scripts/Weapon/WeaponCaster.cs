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

        private void SwitchHand(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _gun.SetActive(!_gun.activeSelf);
        }

        public void Shoot()
        {
            if (_canShoot == false) { return; }

            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            RaycastHit[] hits = Physics.RaycastAll(ray, _camera.farClipPlane);
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

        private void Shoot(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Shoot();
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
                if (parent.TryGetComponent(out Player.NetworkLocalPlayer player))
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

        private void Subscribe()
        {
            if (InputController.Singleton == null) { return; }
            InputController.Singleton.OnHandStarted += SwitchHand;
            InputController.Singleton.OnUseStarted += Shoot;
        }

        private void Unsubscribe()
        {
            if (InputController.Singleton == null) { return; }
            InputController.Singleton.OnHandStarted -= SwitchHand;
            InputController.Singleton.OnUseStarted -= Shoot;
        }

        private void Start()
        {
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}