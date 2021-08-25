using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using MLAPI;
using MLAPI.Messaging;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class NetworkWeaponMessanger : MonoBehaviour
    {
        [SerializeField] private GameObject _serverHitMarkerPrefab = null;
        [SerializeField] private GameObject _clientHitMarkerPrefab = null;
        [SerializeField] private float _hitMarkerLifeTime = 60f;

        public static NetworkWeaponMessanger Singleton { get; private set; } = null;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        #region ServerRPC
        [ServerRpc(RequireOwnership = false)]
        public void HitPlayerServerRpc(ulong playerId, Vector3 localPosition, Quaternion localRotation)
        {
            Transform body = NetworkManager.Singleton
                .ConnectedClients[playerId].PlayerObject
                .GetComponent<NetworkLocalPlayer>().Body;
            if (body == null) { return; }

            HitMarker hitMarker = AddHitMarker(body, localPosition, localRotation);

            HitPlayerClientRpc(playerId, localPosition, localRotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitNpcServerRpc(ulong npcId, Vector3 localPosition, Quaternion localRotation)
        {
            Transform body = FindObjectsOfType<NetworkNPC>().First((npc) => npc.NpcId == npcId).Body;
            if (body == null) { return; }

            HitMarker hitMarker = AddHitMarker(body, localPosition, localRotation);

            HitNpcClientRpc(npcId, localPosition, localRotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitObjectServerRpc(Vector3 position, Quaternion rotation)
        {
            AddHitMarker(null, position, rotation);

            HitObjectClientRpc(position, rotation);
        }
        #endregion ServerRPC

        #region ClientRPC
        [ClientRpc]
        private void HitPlayerClientRpc(ulong playerId, Vector3 localPosition, Quaternion localRotation)
        {
            if (NetworkManager.Singleton.IsHost) { return; }

            Transform body = NetworkManager.Singleton
                .ConnectedClients[playerId].PlayerObject
                .GetComponent<NetworkLocalPlayer>().Body;
            if (body == null) { return; }

            HitMarker hitMarker = AddHitMarker(body, localPosition, localRotation);
        }

        [ClientRpc]
        private void HitNpcClientRpc(ulong npcId, Vector3 localPosition, Quaternion localRotation)
        {
            if (NetworkManager.Singleton.IsHost) { return; }

            Transform body = FindObjectsOfType<NetworkNPC>().First((npc) => npc.NpcId == npcId).Body;
            if (body == null) { return; }

            HitMarker hitMarker = AddHitMarker(body, localPosition, localRotation);
        }

        [ClientRpc]
        private void HitObjectClientRpc(Vector3 position, Quaternion rotation)
        {
            if (NetworkManager.Singleton.IsHost) { return; }

            AddHitMarker(null, position, rotation);
        }
        #endregion ClientRPC

        public HitMarker AddHitMarker(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            GameObject prefab = NetworkManager.Singleton.IsServer ? _serverHitMarkerPrefab : _clientHitMarkerPrefab;
            HitMarker hitMarker = Instantiate(prefab, parent).GetComponent<HitMarker>();
            hitMarker.transform.localPosition = localPosition;
            hitMarker.transform.localRotation = localRotation;
            hitMarker.LifeTime = _hitMarkerLifeTime;
            return hitMarker;
        }
    }
}