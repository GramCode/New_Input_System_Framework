using System.Collections;
using System.Collections.Generic;
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

    PlayerInputActions _input;
    private bool _playerGrounded;

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
    }

    public void MovePlayer(Transform player, float speed, CharacterController controller, Animator anim)
    {
        _playerGrounded = controller.isGrounded;
        var movement = _input.Player.Movement.ReadValue<Vector2>();
        var vertical = movement.y;
        var horizontal = movement.x;

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
}
