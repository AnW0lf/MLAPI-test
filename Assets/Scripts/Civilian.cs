using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
    public class Civilian : MonoBehaviour
    {
        private Animator _animator = null;
        private NavMeshAgent _agent = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_animator != null)
            {
                _animator.SetFloat("xSpeed", _agent.velocity.x);
                _animator.SetFloat("zSpeed", _agent.velocity.z);
            }
        }
    }
}
