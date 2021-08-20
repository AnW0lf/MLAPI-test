using Assets.Scripts.Player;
using MLAPI;
using MLAPI.Messaging;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class NetworkWeaponMessanger : MonoBehaviour
    {
        [SerializeField] private GameObject _hitMarkerPrefab = null;
        [SerializeField] private float _hitMarkerLifeTime = 60f;

        public static NetworkWeaponMessanger Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitPlayerServerRpc(ulong playerId, Vector3 hitPosition, Quaternion hitRotation)
        {
            HitPlayerClientRpc(playerId, hitPosition, hitRotation);
        }

        [ClientRpc]
        private void HitPlayerClientRpc(ulong playerId, Vector3 hitPosition, Quaternion hitRotation)
        {
            Player.NetworkLocalPlayer player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.GetComponent<Player.NetworkLocalPlayer>();
            if (player == null) { return; }

            var hitMarker = Instantiate(_hitMarkerPrefab, player.transform).GetComponent<HitMarker>();
            hitMarker.transform.localPosition = hitPosition;
            hitMarker.transform.localRotation = hitRotation;
            hitMarker.LifeTime = _hitMarkerLifeTime;
            hitMarker.Player = player;
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitCivilianServerRpc(ulong civilianId, Vector3 hitPosition, Quaternion hitRotation)
        {
            HitCivilianClientRpc(civilianId, hitPosition, hitRotation);
        }

        [ClientRpc]
        private void HitCivilianClientRpc(ulong civilianId, Vector3 hitPosition, Quaternion hitRotation)
        {
            Civilian civilian = FindObjectsOfType<Civilian>().First((c) => c.UniqueId == civilianId);
            if (civilian == null) { return; }

            var hitMarker = Instantiate(_hitMarkerPrefab, civilian.transform).GetComponent<HitMarker>();
            hitMarker.transform.localPosition = hitPosition;
            hitMarker.transform.localRotation = hitRotation;
            hitMarker.LifeTime = _hitMarkerLifeTime;
            hitMarker.Civilian = civilian;
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitObjectServerRpc(Vector3 hitPosition, Quaternion hitRotation)
        {
            HitObjectClientRpc(hitPosition, hitRotation);
        }

        [ClientRpc]
        private void HitObjectClientRpc(Vector3 hitPosition, Quaternion hitRotation)
        {
            var hitMarker = Instantiate(_hitMarkerPrefab, null).GetComponent<HitMarker>();
            hitMarker.transform.localPosition = hitPosition;
            hitMarker.transform.localRotation = hitRotation;
            hitMarker.LifeTime = _hitMarkerLifeTime;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SwitchHandServerRpc(ulong clientId)
        {
            SwitchHandClientRpc(clientId);
        }

        [ClientRpc]
        private void SwitchHandClientRpc(ulong clientId)
        {
            var players = NetworkManager.Singleton.ConnectedClientsList
                .Where((client) => client.ClientId != clientId)
                .Select((c) => c.PlayerObject.GetComponent<PlayerItemInHand>());

            foreach(var player in players)
            {
                player.SwitchHand(clientId);
            }
        }
    }
}