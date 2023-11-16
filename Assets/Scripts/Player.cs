using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    private Animator _anim;
    [SerializeField]
    private float _speed = 4f;
    PlayerInputActions _input;
    private bool _playerGrounded;

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();

        _controller = GetComponent<CharacterController>();

        if (_controller == null)
            Debug.LogError("No Character Controller Present");

        _anim = GetComponentInChildren<Animator>();

        if (_anim == null)
            Debug.Log("Failed to connect the Animator");
    }

    void Update()
    {
        //InputManager.Instance.MovePlayer(transform, _speed);
        _playerGrounded = _controller.isGrounded;
        var movement = _input.Player.Movement.ReadValue<Vector2>();
        var vertical = movement.y;
        var horizontal = movement.x;

        var direction = transform.forward * vertical;
        var velocity = direction * _speed;


        _anim.SetFloat("Speed", Mathf.Abs(velocity.magnitude));


        if (_playerGrounded)
            velocity.y = 0f;
        if (!_playerGrounded)
        {
            velocity.y += -20f * Time.deltaTime;
        }

        _controller.Move(velocity * Time.deltaTime);
        transform.Rotate(transform.up * horizontal);
    }
}
