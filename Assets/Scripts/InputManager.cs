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

    public enum ActionMapsEnum
    {
        Player,
        Cameras,
        Drone,
        Forklift
    }

    public enum InputDeviceType
    {
        Keyboard,
        PlayStation,
        Xbox
    }

    private PlayerInputActions _input;
    private bool _playerGrounded;
    private bool _canInteract = false;
    private bool _swapCam = false;
    private bool _escapePressed = false;
    private bool _holdActionPerformed = false;
    private bool _tapActionPerformed = false;

    private float _acceleration = 0f;
    private float _thrust = 0f;
    private float _rotate = 0f;
    private float _strafe = 0f;
    private float _fork;

    private Vector2 _forkliftMovement = new Vector2(0,0);

    private float[] _droneValues = new float[4];
    private float[] _forkliftValues = new float[3];

    private List<InputActionMap> _inputActionMaps;
    
    [SerializeField]
    private string[] _spritesIndexText; //0 = PlayStation button, 1 = Xbox button, 2 = keyboard E key
    private InputDeviceType _inputDevice;

    private bool _gamepadConnected = false;

    private void Awake()
    {
        _instance = this;
        StartCoroutine(CheckForControllers());
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();
            
            if (!_gamepadConnected && controllers.Length > 0)
            {
                _gamepadConnected = true;

                if (Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad)
                {
                    _inputDevice = InputDeviceType.PlayStation;
                }

                if (Gamepad.current is UnityEngine.InputSystem.XInput.XInputController)
                {
                    _inputDevice = InputDeviceType.Xbox;
                }
            }
            else if (_gamepadConnected && controllers.Length == 0)
            {
                _gamepadConnected = false;
                _inputDevice = InputDeviceType.Keyboard;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Interact.started += Interact_started;
        _input.Player.Interact.canceled += Interact_canceled;
        _input.Player.HoldAction.performed += HoldAction_performed;
        _input.Player.TapAction.performed += TapAction_performed;

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

    private void HoldAction_performed(InputAction.CallbackContext context)
    {
        _holdActionPerformed = true;
    }

    private void TapAction_performed(InputAction.CallbackContext context)
    {
        _tapActionPerformed = true;
    }

    public bool Interacted()
    {
        return _canInteract;
    }

    public bool TapPerformed()
    {
        return _tapActionPerformed;
    }

    public bool HoldPerformed()
    {
        return _holdActionPerformed;
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
                _input.Cameras.ExitCameras.performed += ExitCameras_performed;
                break;
            case ActionMapsEnum.Drone:
                _input.Drone.Enable();
                _input.Drone.ExitDrone.performed += ExitDrone_performed;
                break;
            case ActionMapsEnum.Forklift:
                _input.Forklift.Enable();
                _input.Forklift.ExitForklift.performed += ExitForklift_performed;
                break;
                
        }
    }

    private void ExitForklift_performed(InputAction.CallbackContext context)
    {
        _escapePressed = true;
    }

    private void ExitDrone_performed(InputAction.CallbackContext context)
    {
        _escapePressed = true;
    }

    private void Swap_performed(InputAction.CallbackContext context)
    {
        _swapCam = true;
    }

    public void StopSwappingCameras()
    {
        _swapCam = false;
    }

    private void ExitCameras_performed(InputAction.CallbackContext context)
    {
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

    public void ResetHoldInteraction()
    {
        _holdActionPerformed = false;
    }

    public void ResetTap()
    {
        _tapActionPerformed = false;
    }

    public string SpriteToDisplay()
    {
        string name;

        if (_gamepadConnected)
        {
            switch (_inputDevice)
            {
                case InputDeviceType.PlayStation:
                    name = $"{_spritesIndexText[0]}";
                    break;
                case InputDeviceType.Xbox:
                    name = $"{_spritesIndexText[1]}";
                    break;
                default:
                    name = $"{_spritesIndexText[0]}";
                    break;
            }
        }
        else
        {
            name = _spritesIndexText[2];
        }
       
        return name;
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

    public float[] MoveDrone()
    {
        _acceleration = _input.Drone.ForwardBackward.ReadValue<float>();
        _thrust = _input.Drone.Thrust.ReadValue<float>();
        _rotate = _input.Drone.Rotate.ReadValue<float>();
        _strafe = _input.Drone.Strafe.ReadValue<float>();

        _droneValues[0] = _acceleration;
        _droneValues[1] = _thrust;
        _droneValues[2] = _rotate;
        _droneValues[3] = _strafe;

        return _droneValues;
    }

    public float[] MoveForklift()
    {
        _forkliftMovement = _input.Forklift.Move.ReadValue<Vector2>();
        _fork = _input.Forklift.Fork.ReadValue<float>();

        _forkliftValues[0] = _forkliftMovement.x;
        _forkliftValues[1] = _forkliftMovement.y;
        _forkliftValues[2] = _fork;

        return _forkliftValues;
    }

}
