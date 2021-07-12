using MLAPI;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
    public class NetworkCommandLine : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager = null;

        private void Awake()
        {
            if (_networkManager == null)
            {
                _networkManager = GetComponentInParent<NetworkManager>();
            }
        }

        private void Start()
        {
            if (Application.isEditor) return;

            var args = CommandLineArgs;

            if (args.TryGetValue("-mlapi", out string mlapiValue))
            {
                switch (mlapiValue)
                {
                    case "server":
                        _networkManager.StartServer();
                        break;
                    case "host":
                        _networkManager.StartHost();
                        break;
                    case "client":
                        _networkManager.StartClient();
                        break;
                }
            }
        }

        private Dictionary<string, string> CommandLineArgs
        {
            get
            {
                Dictionary<string, string> argDictionary = new Dictionary<string, string>();

                var args = System.Environment.GetCommandLineArgs();

                for (int i = 0; i < args.Length; ++i)
                {
                    var arg = args[i].ToLower();
                    if (arg.StartsWith("-"))
                    {
                        var value = i < arg.Length - 1 ? args[i + 1].ToLower() : null;
                        value = (value?.StartsWith("-") ?? false) ? null : value;

                        argDictionary.Add(arg, value);
                    }
                }

                return argDictionary;
            }
        }
    }
}
