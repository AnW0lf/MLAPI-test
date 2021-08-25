using Assets.Scripts.Weapon;
using MLAPI;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.Player
{
    public class PlayerItemInHand : MonoBehaviour
    {
        [SerializeField] private Rig _rightHandRig = null;
        [SerializeField] private WeaponCaster _ownerGun = null;
        [SerializeField] private GameObject _otherPlayerGun = null;

        private bool _isHandActive = false;

        public void SwitchHand(ulong clientId)
        {
            _isHandActive = !_isHandActive;

            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                _ownerGun.gameObject.SetActive(_isHandActive);
                //NetworkWeaponMessanger.Singleton.SwitchHandServerRpc(clientId);
            }
            else
            {
                _rightHandRig.weight = _isHandActive ? 1f : 0f;
                _otherPlayerGun.SetActive(_isHandActive);
            }
        }

        public void Use()
        {
            if (_ownerGun.gameObject.activeSelf == false) { return; }

            _ownerGun.Shoot();
        }

        private void Update()
        {
            
        }
    }
}