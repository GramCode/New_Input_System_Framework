using System.Collections;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Input Manger Instance is NULL");
            }

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public enum ActionMapsEnum
    {
        Player,
        Cameras,
        Drone
    }

    [SerializeField]
    private InteractableArea _holdArea;
    [SerializeField]
    private GameObject[] _markers;

    private PlayerInputActions _input;
    private bool _playerGrounded;
    private bool _canInteract = false;
    private bool _swapCam = false;
    private bool _escapePressed = false;

    private float _acceleration = 0f;
    private float _thrust = 0f;
    private float _rotate = 0f;
    private float _strafe = 0f;

    private float[] _values = new float[4];

    private List<InputActionMap> _inputActionMaps;

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Interact.started += Interact_started;
        _input.Player.Interact.canceled += Interact_canceled;

        _inputActionMaps = new List<InputActionMap>
        {
            _input.Player,
            _input.Cameras,
            _input.Drone
        };
    }

    private void Interact_started(InputAction.CallbackContext context)
    {
        _canInteract = true;
    }

    private void Interact_canceled(InputAction.CallbackContext context)
    {
        _canInteract = false;
    }

    public bool Interacted()
    {
        return _canInteract;
    }

    public void MovePlayer(Transform player, float speed, CharacterController controller, Animator anim, float rotationMultiplier)
    {
        _playerGrounded = controller.isGrounded;
        var movement = _input.Player.Movement.ReadValue<Vector2>();
        var vertical = movement.y;
        var horizontal = movement.x * rotationMultiplier;

        var direction = player.forward * vertical;
        var velocity = direction * speed;


        anim.SetFloat("Speed", Mathf.Abs(velocity.magnitude));


        if (_playerGrounded)
            velocity.y = 0f;
        if (!_playerGrounded)
        {
            velocity.y += -20f * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
        player.Rotate(player.up * horizontal);

    }

    public void SwapActionMap(ActionMapsEnum actionMap)
    {
        foreach (var map in _inputActionMaps)
        {
            map.Disable();
        }

        switch (actionMap)
        {
            case ActionMapsEnum.Player:
                _input.Player.Enable();
                break;
            case ActionMapsEnum.Cameras:
                _input.Cameras.Enable();
                _input.Cameras.Swap.performed += Swap_performed;
                _input.Cameras.ExitCam.performed += ExitCam_performed;
                break;
            case ActionMapsEnum.Drone:
                _input.Drone.Enable();
                _input.Drone.Exit.performed += Exit_performed;
                break;
        }
    }

    private void Exit_performed(InputAction.CallbackContext context)
    {
        _escapePressed = true;
        SwapActionMap(ActionMapsEnum.Player);
    }

    private void Swap_performed(InputAction.CallbackContext context)
    {
        _swapCam = true;
    }

    public void StopSwappingCameras()
    {
        _swapCam = false;
    }

    private void ExitCam_performed(InputAction.CallbackContext context)
    {
        SwapActionMap(ActionMapsEnum.Player);
        _escapePressed = true;
    }

    public bool SwapCamera()
    {
        return _swapCam;
    }

    public bool BackToPlayer()
    {
        return _escapePressed;
    }

    public void ResetEscape()
    {
        _escapePressed = false;
    }

    public float[] MoveDrone()
    {
        _acceleration = _input.Drone.ForwardBackward.ReadValue<float>();
        _thrust = _input.Drone.Thrust.ReadValue<float>();
        _rotate = _input.Drone.Rotate.ReadValue<float>();
        _strafe = _input.Drone.Strafe.ReadValue<float>();

        _values[0] = _acceleration;
        _values[1] = _thrust;
        _values[2] = _rotate;
        _values[3] = _strafe;

        return _values;
    }
}
