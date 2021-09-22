using UnityEngine;

namespace Assets.Scripts.Lobby
{
    public class PlayerPanel : MonoBehaviour
    {
        [SerializeField] private Transform _container = null;

        public Transform Container => _container;
    }
}