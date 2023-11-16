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

    private void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
    }

    public void MovePlayer(Transform player, float speed)
    {
        var move = _input.Player.Movement.ReadValue<Vector2>();
        player.Translate(new Vector3(move.x, 0, move.y) * Time.deltaTime * speed);
    }
}
