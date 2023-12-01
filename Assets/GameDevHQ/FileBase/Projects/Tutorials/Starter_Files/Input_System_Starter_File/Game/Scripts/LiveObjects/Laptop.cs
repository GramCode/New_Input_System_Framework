using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace Game.Scripts.LiveObjects
{
    public class Laptop : MonoBehaviour
    {
        [SerializeField]
        private Slider _progressBar;
        [SerializeField]
        private int _hackTime = 5;
        private bool _hacked = false;
        [SerializeField]
        private CinemachineVirtualCamera[] _cameras;
        private int _activeCamera = 0;
        [SerializeField]
        private InteractableArea _interactableZone;
        private bool _interacted = false;
        private bool _exitCameras = false;
        private bool _hackComplete = false;

        public static event Action onHackComplete;
        public static event Action onHackEnded;

        private void OnEnable()
        {
            InteractableArea.onHoldStarted += InteractableZone_onHoldStarted;
            InteractableArea.onHoldEnded += InteractableZone_onHoldEnded;
        }

        private void Update()
        {
            if (_hacked == true)
            {
                _interacted = InputManager.Instance.SwapCamera();

                if (_interacted)
                {
                    var previous = _activeCamera;
                    _activeCamera++;


                    if (_activeCamera >= _cameras.Length)
                        _activeCamera = 0;


                    _cameras[_activeCamera].Priority = 11;
                    _cameras[previous].Priority = 9;

                    InputManager.Instance.StopSwappingCameras();
                    _interacted = false;
                }

                _exitCameras = InputManager.Instance.BackToPlayer();

                if (_exitCameras)
                {
                    _interactableZone.HoldPerformed();
                    onHackEnded?.Invoke();
                    _hacked = false;
                    ResetCameras();
                    InputManager.Instance.ResetEscape();
                    InputManager.Instance.SwapActionMap(InputManager.ActionMapsEnum.Player);
                }
            }
        }

        void ResetCameras()
        {
            foreach (var cam in _cameras)
            {
                cam.Priority = 9;
            }
        }

        private void InteractableZone_onHoldStarted(int zoneID)
        {
            if (zoneID == 3 && _hacked == false && _progressBar.value == 0 && _hackComplete == false) //Hacking terminal
            {
                _progressBar.gameObject.SetActive(true);
                StartCoroutine(HackingRoutine());
                onHackComplete?.Invoke();
            }
        }

        private void InteractableZone_onHoldEnded(int zoneID)
        {
            if (zoneID == 3) //Hacking terminal
            {
                if (_hacked == true)
                    return;

                StopAllCoroutines();
                _progressBar.gameObject.SetActive(false);
                _progressBar.value = 0;
                onHackEnded?.Invoke();
            }
        }
        
        IEnumerator HackingRoutine()
        {
            while (_progressBar.value < 1)
            {
                _progressBar.value += Time.deltaTime / _hackTime;
                yield return new WaitForEndOfFrame();
            }
            
            //successfully hacked
            _hacked = true;
            _hackComplete = true;
            _interactableZone.CompleteTask(3);
            //hide progress bar
            _progressBar.gameObject.SetActive(false);
            
            //enable Vcam1
            _cameras[0].Priority = 11;
            //swap action map
            InputManager.Instance.SwapActionMap(InputManager.ActionMapsEnum.Cameras);

        }

        private void OnDisable()
        {
            InteractableArea.onHoldStarted -= InteractableZone_onHoldStarted;
            InteractableArea.onHoldEnded -= InteractableZone_onHoldEnded;
        }
    }

}

