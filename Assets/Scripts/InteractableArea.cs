using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
using Game.Scripts.UI;
using UnityEngine;

public class InteractableArea : MonoBehaviour
{
    private enum ZoneType
    {
        Collectable,
        Action,
        HoldAction
    }

    private enum KeyState
    {
        Press,
        PressHold
    }

    [SerializeField]
    private ZoneType _zoneType;
    [SerializeField]
    private int _zoneID;
    [SerializeField]
    private int _requiredID;
    [SerializeField]
    [Tooltip("Press the (---) Key to .....")]
    private string _displayMessage;
    [SerializeField]
    private GameObject[] _zoneItems;
    [SerializeField]
    private Sprite _inventoryIcon;
    [SerializeField]
    private KeyCode _zoneKeyInput;
    [SerializeField]
    private KeyState _keyState;
    [SerializeField]
    private GameObject _marker;

    private bool _inZone = false;
    private bool _itemsCollected = false;
    private bool _actionPerformed = false;
    private bool _inHoldState = false;
    private bool _holdActionPerformed = false;
    private bool _tapPerformed = false;

    public static event Action<InteractableArea> onZoneInteractionComplete;
    public static event Action<int> onHoldStarted;
    public static event Action<int> onHoldEnded;

    private static int _currentZoneID = 0;
    public static int CurrentZoneID
    {
        get
        {
            return _currentZoneID;
        }
        set
        {
            _currentZoneID = value;

        }
    }

    private void OnEnable()
    {
        InteractableArea.onZoneInteractionComplete += SetMarker;
    }

    private void Update()
    {
        if (_inZone == true)
        {
            if (_zoneID == 6)
            {
                if (InputManager.Instance.TapPerformed() == true)
                {
                    if (_tapPerformed == false)
                    {
                        PerformAction();
                        _tapPerformed = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);
                        
                    }
                }

                else if (InputManager.Instance.HoldPerformed() == true)
                {
                    //perform kick
                    if (_holdActionPerformed == false)
                    {
                        PerformAction();
                        _holdActionPerformed = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);

                    }
                }

            }
            else
            {
                if (InputManager.Instance.Interacted() == true & _keyState != KeyState.PressHold)
                {
                    //press
                    switch (_zoneType)
                    {
                        case ZoneType.Collectable:
                            if (_itemsCollected == false)
                            {
                                CollectItems();
                                _itemsCollected = true;
                                UIManager.Instance.DisplayInteractableZoneMessage(false);
                            }
                            break;

                        case ZoneType.Action:
                            if (_actionPerformed == false)
                            {
                                PerformAction();
                                _actionPerformed = true;
                                UIManager.Instance.DisplayInteractableZoneMessage(false);
                            }
                            break;
                    }
                }
                else if (InputManager.Instance.Interacted() && _keyState == KeyState.PressHold && _inHoldState == false)
                {
                    _inHoldState = true;

                    switch (_zoneType)
                    {
                        case ZoneType.HoldAction:
                            PerformHoldAction();
                            break;
                    }
                }

                if (InputManager.Instance.Interacted() == false && _keyState == KeyState.PressHold && _inHoldState == true)
                {
                    _inHoldState = false;
                    onHoldEnded?.Invoke(_zoneID);
                }
            }
           
        }

    }

    private void CollectItems()
    {
        foreach (var item in _zoneItems)
        {
            item.SetActive(false);
        }

        UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

        CompleteTask(_zoneID);

        onZoneInteractionComplete?.Invoke(this);
    }

    private void PerformAction()
    {
        foreach (var item in _zoneItems)
        {
            item.SetActive(true);
        }

        if (_inventoryIcon != null)
            UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

        onZoneInteractionComplete?.Invoke(this);
    }

    private void PerformHoldAction()
    {
        UIManager.Instance.DisplayInteractableZoneMessage(false);
        onHoldStarted?.Invoke(_zoneID);
    }

    public GameObject[] GetItems()
    {
        return _zoneItems;
    }

    public int GetZoneID()
    {
        return _zoneID;
    }

    public void CompleteTask(int zoneID)
    {
        if (zoneID == _zoneID)
        {
            _currentZoneID++;
            onZoneInteractionComplete?.Invoke(this);
        }
    }

    public void ResetAction(int zoneID)
    {
        if (zoneID == _zoneID)
            _actionPerformed = false;
    }

    public void ResetHoldAction(int zoneID)
    {
        if (zoneID == _zoneID)
        {
            InputManager.Instance.ResetHoldInteraction();
            _holdActionPerformed = false;
        }
    }

    public void ResetTapAction(int zoneID)
    {
        if (zoneID == _zoneID)
        {
            InputManager.Instance.ResetTap();
            _tapPerformed = false;
        }
    }

    private void SetMarker(InteractableArea interactableArea)
    {
        if (_zoneID == _currentZoneID)
            _marker.SetActive(true);
        else
            _marker.SetActive(false);
    }

    public void HoldPerformed()
    {
        _holdActionPerformed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _currentZoneID > _requiredID)
        {
            switch (_zoneType)
            {
                case ZoneType.Collectable:
                    
                    if (_itemsCollected == false)
                    {
                        _inZone = true;

                        if (_displayMessage != null)
                        {
                            string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to collect");
                    }
                    break;

                case ZoneType.Action:
                    if (_actionPerformed == false)
                    {
                        _inZone = true;
                        
                        if (_zoneID == 6)
                        {
                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}. Hold the {_zoneKeyInput.ToString()} key to Kick the Crate";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");

                            ResetHoldAction(6);
                            ResetTapAction(6);
                        }
                        else
                        {
                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                        }
                    }
                    break;
                case ZoneType.HoldAction:
                    if (_holdActionPerformed == false)
                    {
                        _inZone = true;
                        if (_displayMessage != null)
                        {
                            string message = $"Hold the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_zoneKeyInput.ToString()} key to perform action");
                    }

                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inZone = false;
            UIManager.Instance.DisplayInteractableZoneMessage(false);
        }
    }

    private void OnDisable()
    {
        InteractableArea.onZoneInteractionComplete -= SetMarker;
    }
}
