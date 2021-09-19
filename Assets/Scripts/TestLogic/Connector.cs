using MLAPI;
using UnityEngine;

namespace Assets.Scripts.TestLogic
{
    public class Connector : MonoBehaviour
    {
        [SerializeField] private Server _serverPrefab = null;

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) { return; }
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }

        private void OnServerStarted()
        {
            Instantiate(_serverPrefab);
        }
    }
}