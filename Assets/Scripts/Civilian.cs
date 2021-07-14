using UnityEngine;
using MLAPI;
using UnityEngine.AI;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Civilian : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 4f;

    private InterestStage _interestStage = InterestStage.Stay;
    private Transform _target = null;
    private Coroutine _business = null;
    private Coroutine Business
    {
        get => _business;
        set
        {
            if (Business != null) StopCoroutine(_business);
            _business = null;
            _business = value;
        }
    }

    private bool _active = true;
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (_active)
            {
                _agent.enabled = true;
                NextStage(_interestStage);
            }
            else
            {
                Business = null;
                _animator.SetFloat("xSpeed", 0f);
                _animator.SetFloat("zSpeed", 0f);
                _agent.enabled = false;
            }
        }
    }

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(_agent);
            Destroy(this);
        }
    }

    private void Start()
    {
        if (Active)
        {
            Business = StartCoroutine(Stay(Random.Range(0f, 2f)));
        }
    }

    private IEnumerator Stay(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            if (_interestStage != InterestStage.Stay)
            {
                yield break;
            }
            timer += Time.deltaTime;
        }
        NextStage(_interestStage);
    }

    private void NextStage(InterestStage interest)
    {
        switch (interest)
        {
            case InterestStage.WalkTo:
            case InterestStage.RunTo:
                {
                    int r = Random.Range(0, 2);
                    float duration = Random.Range(1f, 10f);
                    if (r == 0)
                    {
                        _interestStage = InterestStage.Stay;
                        Business = StartCoroutine(Stay(duration));
                    }
                    else
                    {
                        _interestStage = InterestStage.LookAround;
                        Business = StartCoroutine(Stay(duration / 2f));
                    }
                }
                break;
            case InterestStage.Stay:
            case InterestStage.LookAround:
                {
                    Business = null;
                    int r = Random.Range(0, 2);
                    if (r == 0)
                    {
                        _interestStage = InterestStage.WalkTo;
                        _target = LevelManager.Singleton.RandomInterestPoint;
                        _agent.speed = _walkSpeed;
                    }
                    else
                    {
                        _interestStage = InterestStage.RunTo;
                        _target = LevelManager.Singleton.RandomInterestPoint;
                        _agent.speed = _runSpeed;
                    }
                }
                break;
        }
    }

    private void Update()
    {
        if (!Active) { return; }

        if (_animator != null)
        {
            _animator.SetFloat("xSpeed", _agent.velocity.x);
            _animator.SetFloat("zSpeed", _agent.velocity.z);
        }

        if (_target != null)
        {
            if (_interestStage == InterestStage.WalkTo || _interestStage == InterestStage.RunTo)
            {
                _agent.SetDestination(_target.position);

                if (Vector3.Distance(transform.position, _target.position) < 1.2f)
                {
                    _target = null;
                    NextStage(_interestStage);
                }
            }
            else
            {
                _target = null;
            }
        }
    }
}

public enum InterestStage { WalkTo, RunTo, Stay, LookAround }
