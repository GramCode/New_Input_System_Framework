using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
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
    [SerializeField]
    private Detonator _detonator;

    private void OnEnable()
    {
        InteractableArea.onZoneInteractionComplete += InteractableArea_onZoneInteractionComplete;
        //Laptop.onHackComplete += ReleasePlayerControl;
        //Laptop.onHackEnded += ReturnPlayerControl;
        //Forklift.onDriveModeEntered += ReleasePlayerControl;
        //Forklift.onDriveModeExited += ReturnPlayerControl;
        //Forklift.onDriveModeEntered += HidePlayer;
        //Drone.OnEnterFlightMode += ReleasePlayerControl;
        //Drone.onExitFlightmode += ReturnPlayerControl;
    }

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

    private void InteractableArea_onZoneInteractionComplete(InteractableArea zone)
    {
        switch (zone.GetZoneID())
        {
            case 1: //place c4
                _detonator.Show();
                break;
            case 2: //Trigger Explosion
                TriggerExplosive();
                break;
        }
    }

    private void TriggerExplosive()
    {
        _detonator.TriggerExplosion();
    }


    private void OnDisable()
    {
        InteractableArea.onZoneInteractionComplete -= InteractableArea_onZoneInteractionComplete;
        //Laptop.onHackComplete -= ReleasePlayerControl;
        //Laptop.onHackEnded -= ReturnPlayerControl;
        //Forklift.onDriveModeEntered -= ReleasePlayerControl;
        //Forklift.onDriveModeExited -= ReturnPlayerControl;
        //Forklift.onDriveModeEntered -= HidePlayer;
        //Drone.OnEnterFlightMode -= ReleasePlayerControl;
        //Drone.onExitFlightmode -= ReturnPlayerControl;
    }
}
