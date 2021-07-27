using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _currentDestination;
    [SerializeField] private float _wanderRange = 10.0f;

    private void Start()
    {
        _agent = gameObject.AddComponent<NavMeshAgent>();
        _agent.speed = 2;
        _agent.angularSpeed = 500;

        _currentDestination = transform.position;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(_currentDestination - transform.position) <= 1)
        {
            if (RandomPointOnNavmesh(_wanderRange, out Vector3 newDestination))
            {
                _currentDestination = newDestination;
                _agent.SetDestination(_currentDestination);
            }
        }
        if (_animator != null)
        {
            _animator.SetFloat("zSpeed", Vector3.Project(_agent.velocity, transform.forward).magnitude);
            _animator.SetFloat("xSpeed", Vector3.Project(_agent.velocity, transform.right).magnitude);
        }
    }

    public void Wander()
    {

    }

    private bool RandomPointOnNavmesh(float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
