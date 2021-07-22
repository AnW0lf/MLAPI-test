using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class Civilian : NetworkBehaviour
{
    private NetworkVariableULong _uniqueId = new NetworkVariableULong(new NetworkVariableSettings
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    }, 0);

    public ulong UniqueId
    {
        get => _uniqueId.Value;
        set
        {
            if (IsServer)
            {
                _uniqueId.Value = value;
            }
            else
            {
                Debug.LogWarning($"{nameof(UniqueId)} can't be changed, because you are not server!..");
            }
        }
    }
}
