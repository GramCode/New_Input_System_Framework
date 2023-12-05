using System.Collections;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

public class StraightenForklift : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Press the (---) Key to .....")]
    private string _displayMessage;
    [SerializeField]
    private KeyCode _zoneKeyInput;
    [SerializeField]
    private Transform _forklift;
    private bool _straightenForklift = false;
    private bool _forkliftStraight = false;

    private void Update()
    {
        if (InputManager.Instance.Interacted() && _straightenForklift)
        {
            Straighten();
            _straightenForklift = false;
            UIManager.Instance.DisplayInteractableZoneMessage(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _forkliftStraight == false)
        {
            CheckRotation();
        }
    }

    private void CheckRotation()
    {
        Vector3 rotation = _forklift.rotation.eulerAngles;
        if (rotation.z > 3)
        {
            ShowMessage();
            _straightenForklift = true;
        }
    }

    private void ShowMessage()
    {
        if (_displayMessage != null)
        {
            string message = $"Press {InputManager.Instance.SpriteToDisplay()} to {_displayMessage}";
            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
        }
        else
            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to straighten");
    }

    private void Straighten()
    {
        _forklift.rotation = new Quaternion(0, 180, 0, 0);
        _forkliftStraight = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            _straightenForklift = false;
        }
    }
}
