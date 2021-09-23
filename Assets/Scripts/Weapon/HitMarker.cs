using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class HitMarker : MonoBehaviour
    {
        public float LifeTime { get; set; } = -1f;

        public NetworkLocalPlayer _player { get; private set; } = null;
        public NetworkNPC _npc { get; private set; } = null;
        private bool _isPlaced = false;
        [SerializeField] private GameObject _raycastObject;
        private float _activationTimer = 0.1f;

        public HitMarketTargetType TargetType
        {
            get
            {
                if (_player != null) return HitMarketTargetType.PLAYER;
                else if (_npc != null) return HitMarketTargetType.NPC;
                else if (_isPlaced) return HitMarketTargetType.PLACE;
                else
                {
                    if (transform.parent == null)
                    {
                        _isPlaced = true;
                        return HitMarketTargetType.PLACE;
                    }
                    else if (transform.parent.TryGetComponent(out LocalPlayer localPlayer))
                    {
                        _player = localPlayer.NetworkParent;
                        return HitMarketTargetType.PLAYER;
                    }
                    else if (transform.parent.TryGetComponent(out RemotePlayer remotePlayer))
                    {
                        _player = remotePlayer.NetworkParent;
                        return HitMarketTargetType.PLAYER;
                    }
                    else if (transform.parent.TryGetComponent(out LocalNPC localNpc))
                    {
                        _npc = localNpc.NetworkParent;
                        return HitMarketTargetType.NPC;
                    }
                    else if (transform.parent.TryGetComponent(out RemoteNPC remoteNpc))
                    {
                        _npc = remoteNpc.NetworkParent;
                        return HitMarketTargetType.NPC;
                    }
                    else
                    {
                        _isPlaced = true;
                        return HitMarketTargetType.PLACE;
                    }
                }
            }
        }

        private void Start()
        {
            if (TargetType != HitMarketTargetType.PLACE)
            {
                LifeTime *= 2f;
            }
        }

        private void Update()
        {
            if (_activationTimer > 0)
            {
                _activationTimer -= Time.deltaTime;
                if (_activationTimer <= 0)
                {
                    _raycastObject.SetActive(true);
                }
            }

            if (LifeTime > 0f)
            {
                LifeTime -= Time.deltaTime;

                if (LifeTime <= 0f)
                {
                    OnEndOfLife();
                }
            }
        }

        private void OnEndOfLife()
        {
            Destroy(gameObject);
        }
    }

    public enum HitMarketTargetType { PLACE, NPC, PLAYER }
}