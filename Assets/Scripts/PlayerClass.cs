using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    private CharacterController _controller;
    private Animator _anim;
    [SerializeField]
    private float _speed = 4f;
    private PlayerInputActions _input;
    [SerializeField]
    private float _rotationMultiplier = 1.5f;

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
        InputManager.Instance.MovePlayer(transform, _speed, _controller, _anim, _rotationMultiplier);
    }
}
