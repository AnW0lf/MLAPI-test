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
    private Vector3 _currentDestination;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _wanderRange = 10.0f;
    [SerializeField] private float _searchRange = 3.0f;

    [SerializeField] private float _changeDestinationDelay = 10f;
    private float _changeDestinationTimer;
    private CopState _currentCopState = CopState.Calm;

    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private GameObject _fieldOfViewMesh;
    [SerializeField] private LineRenderer _targetLine;

    [SerializeField] private Transform _currentTarget;
    public Transform CurrentTarget { get { return _currentTarget; } set { _currentTarget = value; } }
    private AI _currentTargetAI;
    private Transform _currentChaseTarget;
    private Vector3 _lastTargetPosition;
    private Vector3 _lastTargetLookAtPosition;

    [SerializeField] private float _suspiciousDelay;
    private float _suspiciousTimer;

    [SerializeField] private float _searchDelay;
    private float _searchTimer;
    [SerializeField] private int _searchLooksCount;
    private int _searchLooksCurrentCount;
    [SerializeField] private float _searchLookDelay;
    private float _searchLookTimer;
    [SerializeField] private int _searchRunsCount;
    private int _searchRunsCurrentCount;
    private bool _searchDestinationReached;
    private float _lookAtPlayerSearchTimer;

    private float _targetLostTimer = 0.1f;

    [SerializeField] private TextMesh _stateText;
    [SerializeField] private Transform _policeStation;

    private float _findMarksTimer;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        /*_agent.angularSpeed = 1200;
        _agent.acceleration = 100;
        _agent.autoBraking = false;*/
        _currentDestination = transform.position;

        if (_policeStation == null)
        {
            GameObject prison = GameObject.FindGameObjectWithTag("Prison");
            if (prison != null)
            {
                _policeStation = prison.transform;
            }
        }
    
        _changeDestinationTimer = _changeDestinationDelay;

        SwitchToCalm();
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

              /*  if (_animator != null)
                {
                    _animator.SetFloat("velocityX", Vector3.Project(_agent.velocity, transform.forward).magnitude);
                    _animator.SetFloat("velocityY", Vector3.Project(_agent.velocity, transform.right).magnitude);
                }*/

                if (_changeDestinationTimer > 0)
                {
                    _changeDestinationTimer -= Time.deltaTime;
                    if (_changeDestinationTimer <= 0)
                    {
                        SetRandomDestination();
                        _changeDestinationTimer = _changeDestinationDelay;
                    }
                }

                /* if (_currentTarget != null && _currentTargetAI.HasMark == true && _currentTargetAI.UnderArrest == false && _currentTargetAI.InPrison == false && _fieldOfView.CheckTargetVisibility(_currentTarget, 1, true) == true)
                 {                  
                      _currentChaseTarget = _currentTarget;
                      SwitchToSuspicious();
                 } */

                FindMarks();

                break;

            case CopState.Suspicious:

                if (_fieldOfView.CheckTargetVisibility(_currentChaseTarget, 1.5f, false) == false)
                {
                    _targetLostTimer -= Time.deltaTime;
                    if (_targetLostTimer <= 0)
                    {
                        _currentChaseTarget = null;
                        SwitchToCalm();
                        return;
                    }
                }
                else
                {
                    _targetLostTimer = 0.1f;
                }

                transform.LookAt(_currentChaseTarget);
                _targetLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
                _targetLine.SetPosition(1, new Vector3(_currentChaseTarget.position.x, _currentChaseTarget.position.y + 1, _currentChaseTarget.position.z));

                if (_suspiciousTimer >= 0)
                {
                    _suspiciousTimer -= Time.deltaTime;
                    if (_suspiciousTimer <= 0)
                    {
                        SwitchToChase();
                    }
                }

                break;

            case CopState.Chase:

             /*   if (_animator != null)
                {
                    _animator.SetFloat("velocityX", Vector3.Project(_agent.velocity, transform.forward).magnitude);
                    _animator.SetFloat("velocityY", Vector3.Project(_agent.velocity, transform.right).magnitude);
                }*/

                _agent.SetDestination(_currentChaseTarget.position);
                _targetLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
                _targetLine.SetPosition(1, new Vector3(_currentChaseTarget.position.x, _currentChaseTarget.position.y + 1, _currentChaseTarget.position.z));

                if (_fieldOfView.CheckTargetVisibility(_currentChaseTarget, 1.5f, false) == false)
                {
                    _targetLostTimer -= Time.deltaTime;
                    if (_targetLostTimer <= 0)
                    {
                        _currentDestination = _currentChaseTarget.position;
                        SwitchToSearch();
                    }
                }
                else
                {
                    _targetLostTimer = 0.1f;
                }

                if (Vector3.SqrMagnitude(_currentChaseTarget.position - transform.position) < 4)
                {
                    SwitchToArrest();
                }

                break;

            case CopState.Search:

               /* if (_animator != null)
                {
                    _animator.SetFloat("velocityX", Vector3.Project(_agent.velocity, transform.forward).magnitude);
                    _animator.SetFloat("velocityY", Vector3.Project(_agent.velocity, transform.right).magnitude);
                }*/

                if (_lookAtPlayerSearchTimer > 0)
                {
                    _lookAtPlayerSearchTimer -= Time.deltaTime;
                    _lastTargetLookAtPosition = _currentChaseTarget.transform.position;
                }

                if (Vector3.SqrMagnitude(_currentDestination - transform.position) <= 1f)
                {                  
                    if (_searchDestinationReached == false)
                    {                       
                        _searchDestinationReached = true;
                        _agent.SetDestination(_lastTargetLookAtPosition);                       
                    }
                    else if (_searchLooksCurrentCount > 0 && _searchLookTimer > 0)
                    {
                        _searchLookTimer -= Time.deltaTime;
                        if (_searchLookTimer <= 0)
                        {
                            if (RandomPointOnNavmesh(_agent.transform.position, 1.5f, out Vector3 newDestination))
                            {
                                _currentDestination = newDestination;
                                _agent.SetDestination(_currentDestination);
                            }

                            _searchLookTimer = _searchLookDelay;
                            --_searchLooksCurrentCount;                                              
                        }
                    }
                    
                    if (_searchLooksCurrentCount <= 0)
                    {
                        if (RandomPointOnNavmesh(_lastTargetPosition, _searchRange, out Vector3 newDestination))
                        {
                            _currentDestination = newDestination;
                            _agent.SetDestination(_currentDestination);
                        }
                    }                 
                }

                if (_fieldOfView.CheckTargetVisibility(_currentTarget, 1.5f, true) == true)
                {
                    _currentChaseTarget = _currentTarget;
                    SwitchToChase();
                }

                if (_searchDestinationReached == true && _searchTimer > 0)
                {
                    _searchTimer -= Time.deltaTime;
                    if (_searchTimer <= 0)
                    {
                        SwitchToCalm();
                    }
                }

                break;

            case CopState.Arrest:

                _agent.SetDestination(_policeStation.position);
                if (Vector3.SqrMagnitude(_policeStation.position - transform.position) < 4)
                {
                    _currentChaseTarget.GetComponent<AI>().GoToPrison(_policeStation.transform);
                    SwitchToCalm();
                }

                break;

            default:
                break;
        }

        _stateText.transform.rotation = Quaternion.identity;

        if (_findMarksTimer > 0)
        {
            _findMarksTimer -= Time.deltaTime;
        }
    }

    private void SwitchToCalm()
    {
        _currentCopState = CopState.Calm;
        _fieldOfViewMesh.SetActive(true);
        _suspiciousTimer = 0;
        
        _agent.speed = _walkSpeed;
        _stateText.text = "...";

        _targetLine.SetPosition(0, transform.position);
        _targetLine.SetPosition(1, transform.position);
        _targetLine.startColor = Color.yellow;
        _targetLine.endColor = Color.yellow;

        _currentDestination = transform.position;
    }

    private void SwitchToSuspicious()
    {
        _currentCopState = CopState.Suspicious;
        _agent.speed = 0;
       // _animator.SetFloat("velocityX", 0);
       // _animator.SetFloat("velocityY", 0);
        _suspiciousTimer = _suspiciousDelay;
        _stateText.text = "?";
        _targetLostTimer = 0.1f;
    }

    private void SwitchToChase()
    {
        _currentCopState = CopState.Chase;
        _agent.speed = _runSpeed;
        _stateText.text = "!!!";

        _targetLine.startColor = Color.red;
        _targetLine.endColor = Color.red;
        _targetLostTimer = 0.1f;
    }

    private void SwitchToSearch()
    {
        _currentCopState = CopState.Search;
        _targetLine.SetPosition(0, transform.position);
        _targetLine.SetPosition(1, transform.position);
        _stateText.text = "?!";
        _agent.SetDestination(_currentChaseTarget.position);
        _lastTargetPosition = _currentChaseTarget.position;      
        _searchTimer = _searchDelay;
        _lookAtPlayerSearchTimer = 0.75f;

        _searchLooksCurrentCount = _searchLooksCount;
        _searchRunsCount = _searchRunsCurrentCount;
        _searchLookTimer = _searchLookDelay;
        _searchDestinationReached = false;
    }

    private void SwitchToArrest()
    {
        _currentCopState = CopState.Arrest;

        _currentTargetAI.GetComponent<CitizenNPC>().enabled = false;
        _currentTargetAI.enabled = true;

        _currentTargetAI.UnderArrest = true;
        _currentTargetAI.OwningCopTransform = transform;
        _agent.speed = _walkSpeed;
        _fieldOfViewMesh.SetActive(false);

        _targetLine.SetPosition(0, transform.position);
        _targetLine.SetPosition(1, transform.position);
        _targetLine.startColor = Color.yellow;
        _targetLine.endColor = Color.yellow;

        _stateText.text = "Now pray, scum";
    }

    private void SetRandomDestination()
    {    
        if (RandomPointOnNavmesh(transform.position, _wanderRange, out Vector3 newDestination))
        {
            _currentDestination = newDestination;
            _agent.SetDestination(_currentDestination);
        }
    }

    private bool RandomPointOnNavmesh(Vector3 startPoint, float range, out Vector3 result)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPoint = startPoint + Random.insideUnitSphere * range;

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

    private void FindMarks()
    {
        if (_currentTarget != null || _findMarksTimer > 0)
        {
            return;
        }

        Collider[] neighbors = Physics.OverlapSphere(transform.position, _fieldOfView.viewRadius);
        foreach(Collider tempCollider in neighbors)
        {
            if (tempCollider.tag == "Mark")
            {               
                if (_fieldOfView.CheckTargetVisibility(tempCollider.transform, 1.5f, true) == true)
                {
                    Debug.Log("FOUND");
                    AI tempAI = tempCollider.transform.GetComponentInParent<AI>();
                    if (tempAI != null && tempAI.UnderArrest == false)
                    {                                       
                        Debug.Log("AI");
                        CurrentTarget = tempAI.transform;
                        _currentChaseTarget = _currentTarget;
                        _currentTargetAI = tempAI;
                        SwitchToSuspicious();
                        break;
                    }                   
                }                    
            }
        }

        _findMarksTimer = 0.2f;
    }
}
