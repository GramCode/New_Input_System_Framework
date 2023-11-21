using System.Collections;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

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

    [SerializeField]
    private InteractableArea _area;
    [SerializeField]
    private GameObject[] _markers;

    private PlayerInputActions _input;
    private bool _playerGrounded;
    private bool _canInteract = false;
    private bool _itemColected = false;
    private bool _actionPerformed = false;
    private bool _fillProgressBar = false;

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Interact.started += Interact_started;
        _input.Player.Interact.canceled += Interact_canceled;
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _canInteract = true;
        _fillProgressBar = true;
    }

    private void Interact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _canInteract = false;
        _fillProgressBar = false;
    }

    public bool PickUpItem(GameObject[] zoneItems, Sprite icon, int zoneID, InteractableArea interactableArea)
    {
        if (_canInteract == true && _itemColected == false)
        {
            foreach (var item in zoneItems)
            {
                item.SetActive(false);
            }
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            UIManager.Instance.UpdateInventoryDisplay(icon);
            _itemColected = true;
            interactableArea.CompleteTask(zoneID);
            interactableArea.InteractionPerformed();
            
        }

        return _itemColected;
    }

    public bool PerformAction(GameObject[] zoneItems, Sprite inventoryIcon, InteractableArea interactableArea)
    {
        if (_canInteract == true)
        {
            //_canInteract = false;
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            _actionPerformed = true;

            foreach (var item in zoneItems)
            {
                item.SetActive(true);
            }

            if (inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(inventoryIcon);
            interactableArea.InteractionPerformed();
            
        }

        return _actionPerformed;
    }

    public void ResetActionPerformed()
    {
        _actionPerformed = false;
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

    public bool FillProgressBar()
    {
        return _fillProgressBar;
    }

}
