using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private int counter = 0;

        public override void NetworkStart()
        {
            counter++;
            print($"Network start:: {NetworkManager.Singleton.LocalClientId} COUNT: {counter}");
        }
    }
}