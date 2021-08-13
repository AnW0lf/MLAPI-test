using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cop : MonoBehaviour
{
    public enum CopState
    {
        Calm,
        Suspicious,
        Chase,
        Search,
        Arrest
    }

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _currentDestination;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _wanderRange = 10.0f;

    [SerializeField] private float _changeDestinationDelay = 10f;
    private float _changeDestinationTimer;
    private CopState _currentCopState = CopState.Calm;

    [SerializeField] private FieldOfView _fieldOfView;

    [SerializeField] private Transform _debugTarget;
    private Transform _currentChaseTarget;

    private void Start()
    {
        _agent = gameObject.AddComponent<NavMeshAgent>();
        _agent.speed = _walkSpeed;
        _agent.angularSpeed = 500;
        _currentDestination = transform.position;

        _animator = GetComponent<Animator>();
        _changeDestinationTimer = _changeDestinationDelay;
    }

    
    private void Update()
    {
        switch(_currentCopState)
        {
            case CopState.Calm:

                if (Vector3.SqrMagnitude(_currentDestination - transform.position) <= 1)
                {
                    SetRandomDestination();
                }

                if (_animator != null)
                {
                    _animator.SetFloat("zSpeed", Vector3.Project(_agent.velocity, transform.forward).magnitude);
                    _animator.SetFloat("xSpeed", Vector3.Project(_agent.velocity, transform.right).magnitude);
                }

                if (_changeDestinationTimer > 0)
                {
                    _changeDestinationTimer -= Time.deltaTime;
                    if (_changeDestinationTimer <= 0)
                    {
                        SetRandomDestination();
                        _changeDestinationTimer = _changeDestinationDelay;
                    }
                }

                if (_fieldOfView.CheckTargetVisibility(_debugTarget, true) == true)
                {
                    _currentChaseTarget = _debugTarget;
                    SwitchToChase();
                }

                break;

            case CopState.Suspicious:

                break;

            case CopState.Chase:

                _agent.SetDestination(_currentChaseTarget.position);

                break;

            case CopState.Search:

                break;

            case CopState.Arrest:

                break;

            default:
                break;
        }
    }

    private void SwitchToCalm()
    {

    }

    private void SwitchToSuspicious()
    {

    }

    private void SwitchToChase()
    {
        _currentCopState = CopState.Chase;
        _agent.speed = _runSpeed;
    }

    private void SwitchToSearch()
    {

    }

    private void SwitchToArrest()
    {

    }

    private void SetRandomDestination()
    {
        if (RandomPointOnNavmesh(_wanderRange, out Vector3 newDestination))
        {
            _currentDestination = newDestination;
            _agent.SetDestination(_currentDestination);
        }
    }

    private bool RandomPointOnNavmesh(float range, out Vector3 result)
    {
        for (int i = 0; i < 3; i++)
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
