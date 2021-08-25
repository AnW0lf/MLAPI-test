using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LocalPlayer : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody = null;
        public NetworkLocalPlayer NetworkParent { get; set; } = null;

        public Vector3 Velocity => _rigidbody.velocity;
    }
}