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

        private void SwitchHand(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _gun.SetActive(!_gun.activeSelf);
        }

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
            ulong id = ulong.MaxValue;
            Vector3 position = hit.point + hit.normal * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(-hit.normal, Vector3.forward);
            Transform target = hit.transform;

            if (NetworkWeaponMessanger.Singleton != null)
            {
                if (target.TryGetComponent(out RemotePlayer remotePlayer))
                {
                    id = remotePlayer.NetworkParent.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitPlayerServerRpc(id, position, rotation);
                }
                else if (target.TryGetComponent(out RemoteNPC remoteNpc))
                {
                    id = remoteNpc.NetworkParent.NpcId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitNpcServerRpc(id, position, rotation);

                }
                else if (target.TryGetComponent(out LocalPlayer localPlayer))
                {
                    id = localPlayer.NetworkParent.OwnerClientId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitPlayerServerRpc(id, position, rotation);
                }
                else if (target.TryGetComponent(out LocalNPC localNpc))
                {
                    id = localNpc.NetworkParent.NpcId;
                    position = target.InverseTransformPoint(position);
                    rotation = target.rotation * rotation;
                    NetworkWeaponMessanger.Singleton.HitNpcServerRpc(id, position, rotation);
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