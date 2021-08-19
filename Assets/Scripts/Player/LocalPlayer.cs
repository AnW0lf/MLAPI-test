using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LocalPlayer : MonoBehaviour
    {
        [SerializeField] private FirstPersonAIO _controller = null;

        public Vector3 Velocity => _controller.Velocity;
    }
}