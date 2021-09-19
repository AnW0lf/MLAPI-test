using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _currentDestination;
    [SerializeField] private float _wanderRange = 10.0f;
    [SerializeField] private bool _directControlEnabled;
    [SerializeField] private float _speed;

    private InputManager _inputManager;

    public Transform OwningCopTransform { get; set; }
    public bool HasMark { get; set; }
    public bool UnderArrest { get; set; }
    public bool InPrison { get; set; }

    private void Start()
    {
        if (_directControlEnabled == true)
        {
            _inputManager = new InputManager();
            _inputManager.Game.Use.performed += SetDestination;
            _inputManager.Game.Hand.performed += WarpToDestination;
            _inputManager.Game.Enable();
        }

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        _currentDestination = transform.position;
    }

    private void Update()
    {       
        if (InPrison == true)
        {          
            return;
        }

        if (UnderArrest == true)
        {
            _agent.SetDestination(OwningCopTransform.position);
        }
        else if (_directControlEnabled == false && Vector3.SqrMagnitude(_currentDestination - transform.position) <= 1)
        {
            if (RandomPointOnNavmesh(_wanderRange, out Vector3 newDestination))
            {
                _currentDestination = newDestination;
                _agent.SetDestination(_currentDestination);
            }
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

    public void SetDestination(InputAction.CallbackContext context)
    {      
        if (UnderArrest == true)
        {
            return;
        }

        RaycastHit hit;
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());     

        if (Physics.Raycast(ray, out hit))
        {
            _agent.SetDestination(hit.point);
        }
    }

    public void WarpToDestination(InputAction.CallbackContext context)
    {
        if (UnderArrest == true)
        {
            return;
        }

        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit))
        {
            _agent.Warp(hit.point);
        }
    }

    public void GoToPrison(Transform prisonTransform)
    {
        _agent.speed = _speed;
        _agent.SetDestination(prisonTransform.position);
        InPrison = true;
        UnderArrest = false;
    }
}
