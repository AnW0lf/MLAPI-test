using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _currentDestination;
    [SerializeField] private float _wanderRange = 10.0f;

    private void Start()
    {
        _agent = gameObject.AddComponent<NavMeshAgent>();
        RandomPointOnNavmesh(_wanderRange, out _currentDestination);
        _agent.SetDestination(_currentDestination);

        _agent.speed = 2;
        _agent.angularSpeed = 500;

        _animator = GetComponent<Animator>();
        _animator.SetFloat("zSpeed", 1);
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(_currentDestination - transform.position) <= 1)
        {
            RandomPointOnNavmesh(_wanderRange, out _currentDestination);
            _agent.SetDestination(_currentDestination);
            _animator.SetFloat("zSpeed", 1);
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
