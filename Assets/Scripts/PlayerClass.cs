using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    private float _rotationMultiplier = 3f;
    [SerializeField]
    private Detonator _detonator;
    private bool _canMove = true;
    [SerializeField]
    private CinemachineVirtualCamera _followCam;
    [SerializeField]
    private GameObject _model;

    private void OnEnable()
    {
        InteractableArea.onZoneInteractionComplete += InteractableArea_onZoneInteractionComplete;
        Laptop.onHackComplete += ReleasePlayerControl;
        Laptop.onHackEnded += ReturnPlayerControl;
        Forklift.onDriveModeEntered += ReleasePlayerControl;
        Forklift.onDriveModeExited += ReturnPlayerControl;
        Forklift.onDriveModeEntered += HidePlayer;
        Drone.OnEnterFlightMode += ReleasePlayerControl;
        Drone.onExitFlightmode += ReturnPlayerControl;
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
        if (_canMove)
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

    private void ReleasePlayerControl()
    {
        _canMove = false;
        _followCam.Priority = 9;
    }

    private void ReturnPlayerControl()
    {
        _model.SetActive(true);
        _canMove = true;
        _followCam.Priority = 10;
    }

    private void HidePlayer()
    {
        _model.SetActive(false);
    }

    private void TriggerExplosive()
    {
        _detonator.TriggerExplosion();
    }

    public void PuchAnim()
    {
        _anim.SetTrigger("Punch");
    }

    public void KickAnim()
    {
        _anim.SetTrigger("Kick");
    }

    private void OnDisable()
    {
        InteractableArea.onZoneInteractionComplete -= InteractableArea_onZoneInteractionComplete;
        Laptop.onHackComplete -= ReleasePlayerControl;
        Laptop.onHackEnded -= ReturnPlayerControl;
        Forklift.onDriveModeEntered -= ReleasePlayerControl;
        Forklift.onDriveModeExited -= ReturnPlayerControl;
        Forklift.onDriveModeEntered -= HidePlayer;
        Drone.OnEnterFlightMode -= ReleasePlayerControl;
        Drone.onExitFlightmode -= ReturnPlayerControl;
    }
}
