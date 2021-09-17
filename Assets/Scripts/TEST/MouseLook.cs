using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private Transform _playerTransform;
    private float _xRotation = 0;

    private void Start()
    {
        Cursor.lockState =  CursorLockMode.Locked;
    }
    public void ProcessMouse(float mouseX, float mouseY)
    {
        mouseX *= _mouseSensitivity * Time.deltaTime;
        mouseY *= _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerTransform.Rotate(Vector3.up * mouseX);
    }
}
