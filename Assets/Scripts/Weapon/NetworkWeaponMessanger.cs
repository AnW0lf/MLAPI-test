using Assets.Scripts.Network;
using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using MLAPI;
using MLAPI.Messaging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class NetworkWeaponMessanger : NetworkBehaviour
    {
        [SerializeField] private GameObject _serverHitMarkerPrefab = null;
        [SerializeField] private GameObject _clientHitMarkerPrefab = null;
        [SerializeField] private float _hitMarkerLifeTime = 60f;

        public static NetworkWeaponMessanger Singleton { get; private set; } = null;

        private readonly Dictionary<ulong, NetworkActor> _actors = new Dictionary<ulong, NetworkActor>();
        private void FindActors()
        {
            foreach(var actor in FindObjectsOfType<NetworkActor>())
            {
                if (_actors.ContainsValue(actor) == false)
                {
                    _actors.Add(actor.ID.Value, actor);
                }
            }
        }

        private void Awake()
        {
            _actors.Clear();
            if (Singleton == null) Singleton = this;
            else if (Singleton != this) Destroy(gameObject);
        }

        #region ServerRPC
        [ServerRpc(RequireOwnership = false)]
        public void HitActorServerRpc(ulong actorId, Vector3 localPosition, Quaternion localRotation)
        {
            if (_actors.ContainsKey(actorId)) FindActors();
            HitActorClientRpc(actorId, localPosition, localRotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HitObjectServerRpc(Vector3 position, Quaternion rotation)
        {
            HitObjectClientRpc(position, rotation);
        }
        #endregion ServerRPC

        #region ClientRPC
        [ClientRpc]
        private void HitActorClientRpc(ulong actorId, Vector3 localPosition, Quaternion localRotation)
        {
            if (_actors.ContainsKey(actorId) == false) FindActors();

            Transform body = _actors[actorId].BodyArchitector.Body;

            if (body == null) { return; }

            AddHitMarker(body, localPosition, localRotation);
        }

        [ClientRpc]
        private void HitObjectClientRpc(Vector3 position, Quaternion rotation)
        {
            AddHitMarker(null, position, rotation);
        }
        #endregion ClientRPC

        public HitMarker AddHitMarker(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            print($"Add hit {localPosition}");
            GameObject prefab = NetworkManager.Singleton.IsServer ? _serverHitMarkerPrefab : _clientHitMarkerPrefab;
            HitMarker hitMarker = Instantiate(prefab, parent).GetComponent<HitMarker>();
            hitMarker.transform.localPosition = localPosition;
            hitMarker.transform.localRotation = localRotation;
            hitMarker.LifeTime = _hitMarkerLifeTime;
            return hitMarker;
        }
    }
}