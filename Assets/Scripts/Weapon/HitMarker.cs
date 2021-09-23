using Assets.Scripts.Network;
using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class HitMarker : MonoBehaviour
    {
        public float LifeTime { get; set; } = -1f;

        public NetworkActor _actor { get; private set; } = null;
        public NetworkNPC _npc { get; private set; } = null;
        private bool _isPlaced = false;
        [SerializeField] private GameObject _raycastObject;
        private float _activationTimer = 0.1f;

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

    public enum HitMarketTargetType { PLACE, ACTOR }
}