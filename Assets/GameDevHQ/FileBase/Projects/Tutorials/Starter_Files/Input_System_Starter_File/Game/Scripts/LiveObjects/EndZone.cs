using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.LiveObjects
{
    public class EndZone : MonoBehaviour
    {
        private void OnEnable()
        {
            InteractableArea.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableArea zone)
        {
            if (zone.GetZoneID() == 7)
            {
                InteractableArea.CurrentZoneID = 0;
                SceneManager.LoadScene(0);
            }
        }

        private void OnDisable()
        {
            InteractableArea.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}