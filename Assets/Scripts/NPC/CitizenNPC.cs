using UnityEngine;
using UnityEngine.AI;

public class CitizenNPC : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent = null;
    [SerializeField] private float _searchRange = 5f;
    [Header("Error settings")]
    [SerializeField] private float _offsetError = 0.2f;
    [SerializeField] private float _timeError = 5f;

    private CitizenNPCState _state = CitizenNPCState.STAY;
    private Vector3 _oldPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    private float _errorTimer = 0f;

    private void Update()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        switch (_state)
        {
            case CitizenNPCState.FOLLOW:
                OnFollow();
                break;
            case CitizenNPCState.STAY:
                OnStay();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_targetPosition, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _targetPosition);
    }

    private void Search()
    {
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(_searchRange * 0.1f, _searchRange);
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _searchRange, 1);
        _targetPosition = hit.position;

        _agent.SetDestination(_targetPosition);
        _state = CitizenNPCState.FOLLOW;
    }

    private void OnFollow()
    {
        if (Vector3.Distance(transform.position, _targetPosition) < _offsetError)
        {
            _state = CitizenNPCState.STAY;
        }
        else
        {
            if (Vector3.Distance(transform.position, _oldPosition) < _agent.speed * 0.5f * Time.deltaTime)
            {
                _errorTimer += Time.deltaTime;

                if (_errorTimer > _timeError)
                {
                    _errorTimer = 0f;
                    _state = CitizenNPCState.STAY;
                }
            }
            else
            {
                _errorTimer = 0f;
            }
            _oldPosition = transform.position;
        }

    }

    private void OnStay()
    {
        Search();
    }

    private enum CitizenNPCState { FOLLOW, STAY }
}
