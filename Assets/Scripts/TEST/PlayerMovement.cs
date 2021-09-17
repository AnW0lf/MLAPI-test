using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speedChangeLerpSpeed;
    [SerializeField] private float _speed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _crouchSpeed;
    private float _currentSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpHeight = 1f;

    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _groundLayerMask;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _weaponBobTransform;
    [SerializeField] private float _crouchLerpSpeed;
    [SerializeField] private float _jumpBobDuration;
    private float _jumpBobTimer;

    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isCrouched;
    private bool _isRunning;

    private void Start()
    {
        _currentSpeed = _speed;
    }

    public void ProcessMovement(float x, float z, bool run, bool jump, bool crouch, bool shooting)
    {
        bool previousGrounded = _isGrounded;

        _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckDistance, _groundLayerMask);
        _isCrouched = crouch;
        _isRunning = run;

        if (crouch == true && _isGrounded == true)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _crouchSpeed, _speedChangeLerpSpeed * Time.deltaTime);
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, new Vector3(0, -0.4f, _cameraTransform.localPosition.z), _crouchLerpSpeed * Time.deltaTime); 
        }
        else
        {
            _currentSpeed = run == true ? Mathf.Lerp(_currentSpeed, _runSpeed, _speedChangeLerpSpeed * Time.deltaTime) : Mathf.Lerp(_currentSpeed, _speed, _speedChangeLerpSpeed * Time.deltaTime);
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, new Vector3(0, 0.2f, _cameraTransform.localPosition.z), _crouchLerpSpeed * Time.deltaTime);
        }

        if (_isGrounded == true && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (jump == true && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _jumpBobTimer = 0;
        }

        _velocity.y += _gravity * Time.deltaTime;

        Vector3 movementVector = (transform.right * x * _currentSpeed * Time.deltaTime) + _velocity * Time.deltaTime + (transform.forward * z * _currentSpeed * Time.deltaTime);
        _controller.Move(movementVector);

        if ((x != 0 || z != 0) && _isGrounded == true)
        {
            ProcessHeadBob();
            ProcessWeaponBob();                
        }
        else
        {        
            _weaponBobTransform.localPosition = Vector3.MoveTowards(_weaponBobTransform.localPosition, Vector3.zero, 0.4f * Time.deltaTime);
        }

        if (previousGrounded == false && _isGrounded == true)
        {
            _jumpBobTimer = 0.3f;
        }

        if (_jumpBobTimer > 0)
        {
            _jumpBobTimer -= Time.deltaTime;
            ProcessJumpBob();
        }
    }

    public void ProcessHeadBob()
    {
        float xMult = 7f;
        float yMult = 12.5f;

        if (_isCrouched == true)
        {
            xMult = 4f;
            yMult = 6f;
        }

        if (_isRunning == true)
        {
            xMult = 10f;
            yMult = 20f;
        }

        float xShift = Mathf.Sin(Time.time * xMult) * 1.2f * Time.deltaTime;
        float yShift = Mathf.Sin(Time.time * yMult) * 1.2f * Time.deltaTime;
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x + xShift, _cameraTransform.localPosition.y + yShift, _cameraTransform.localPosition.z);
    }

    public void ProcessWeaponBob()
    {
        float xMult = 7f;
        float yMult = 12.5f;

        if (_isCrouched == true)
        {
            xMult = 3f;
            yMult = 5f;
        }

        if (_isRunning == true)
        {
            xMult = 10f;
            yMult = 20f;
        }

        float xShift = Mathf.Sin(Time.time * xMult) * 0.2f * Time.deltaTime;
        float yShift = Mathf.Sin(Time.time * yMult) * 0.2f * Time.deltaTime;
        _weaponBobTransform.localPosition = new Vector3(_weaponBobTransform.localPosition.x + xShift, _weaponBobTransform.localPosition.y + yShift, _weaponBobTransform.localPosition.z);
    }

    public void ProcessJumpBob()
    {
        float yMult = 5f;

        float yShift = Mathf.Sin(_jumpBobTimer * -yMult) * 5.5f * Time.deltaTime;
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x, _cameraTransform.localPosition.y + yShift, _cameraTransform.localPosition.z);
    }
}
