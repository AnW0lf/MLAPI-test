using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        #region Position
        private NetworkVariableVector3 NV_Position = new NetworkVariableVector3(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public event UnityAction<Vector3> OnPositionChanged = null;

        public Vector3 Position
        {
            get
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    return transform.position;
                }
                else
                {
                    return NV_Position.Value;
                }
            }
            set
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    transform.position = value;
                    NV_Position.Value = value;
                }
                else
                {
                    SubmitPositionRequestServerRpc(value);
                }
                OnPositionChanged?.Invoke(value);
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
        {
            NV_Position.Value = position;
        }
        #endregion Position

        #region Rotation
        private NetworkVariableQuaternion NV_Rotation = new NetworkVariableQuaternion(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });

        public event UnityAction<Quaternion> OnRotationChanged = null;

        public Quaternion Rotation
        {
            get
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    return transform.rotation;
                }
                else
                {
                    return NV_Rotation.Value;
                }
            }
            set
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    transform.rotation = value;
                    NV_Rotation.Value = value;
                }
                else
                {
                    SubmitRotationRequestServerRpc(value);
                }

                OnRotationChanged?.Invoke(value);
            }
        }

        [ServerRpc]
        void SubmitRotationRequestServerRpc(Quaternion rotation, ServerRpcParams rpcParams = default)
        {
            NV_Rotation.Value = rotation;
        }
        #endregion Rotation
    }
}