using Game;
using MLAPI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class HitMarker : MonoBehaviour
    {
        public float LifeTime { get; set; } = -1f;
        public Player.NetworkLocalPlayer Player { get; set; } = null;
        public CitizenNPC Civilian { get; set; } = null;

        private void Update()
        {
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
}