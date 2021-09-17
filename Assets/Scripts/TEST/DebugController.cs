using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    private static DebugController _instance;
    public static DebugController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DebugController>();
            }
            return _instance;
        }
    }

    [SerializeField] private MouseLook _mouseLook;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Weapon _playerGun;

    private void Update()
    {
        bool shooting = Input.GetMouseButton(0);

        _mouseLook.ProcessMouse(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _playerMovement.ProcessMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.LeftShift), Input.GetButtonDown("Jump"), Input.GetKey(KeyCode.LeftControl), shooting);

        if (shooting == true)
        {
            _playerGun.RaycastShoot();
        }
    }
}
