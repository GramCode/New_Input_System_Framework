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
        Action
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
    private GameObject _marker;

    private bool _inZone = false;
    private bool _itemsCollected = false;
    private bool _actionPerformed = false;
    private bool _carExploded = false;

    public static event Action<InteractableArea> onZoneInteractionComplete;
    
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

    /// <summary>
    /// The update method is going to constantly check for the zone type
    /// 
    /// If we are in a collectible zone type and the item is not collected
    /// Once we perform the input action the item will be collected, (it will become invisible in the scene
    /// and it will become visible in your inventory)
    ///
    /// If we are in an action zone type, once we perform the input action the interaction will be performed.
    /// 
    /// Completing a task will increment the current zone id and set the marker visible for the next zone
    ///
    /// If we are in the zone to detonate the C4, once the action is performed and explosion will be instantiated
    /// and we'll complete all the interactions
    /// </summary>
    private void Update()
    {
        switch (_zoneType)
        {
            case ZoneType.Collectable:
                if (_inZone == true && _itemsCollected == false)
                {
                    _itemsCollected = InputManager.Instance.PickUpItem(_zoneItems, _inventoryIcon, this);
                }
                break;
            case ZoneType.Action:
                if (_inZone == true && _zoneID == _currentZoneID)
                {
                    _actionPerformed = InputManager.Instance.PerformAction(_zoneItems, _inventoryIcon, this);

                    if (_actionPerformed == true && _zoneID == 2 && _carExploded == false)
                    {
                        _carExploded = true;
                        GameObject.Find("TaxiExplosion").GetComponent<TaxiExplosion>().Explode();
                    }

                    InputManager.Instance.ResetActionPerformed();
                }
                break;
        }

    }

    //This function is called everytime we perform an interaction
    public void InteractionPerformed()
    {
        CompleteTask(_zoneID);
    }

    private void CompleteTask(int zoneID)
    {
        if (zoneID == _zoneID)
        {
            _currentZoneID++;
            onZoneInteractionComplete?.Invoke(this);
        }
    }

    private void SetMarker(InteractableArea interactableArea)
    {
        if (_zoneID == _currentZoneID)
            _marker.SetActive(true);
        else
            _marker.SetActive(false);
    }

    //Display the inteaction message once in an interactable zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _zoneID > _requiredID)
        {
            _inZone = true;
            
            switch (_zoneType)
            {
                case ZoneType.Collectable:
                    
                    if (_itemsCollected == false)
                    {
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

                    if (_actionPerformed == false && _zoneID == _currentZoneID)
                    {
                        if (_displayMessage != null)
                        {
                            string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                    }
                    break;
            }
        }
    }

    //Hides the interactable message once out of an interactable zone
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
