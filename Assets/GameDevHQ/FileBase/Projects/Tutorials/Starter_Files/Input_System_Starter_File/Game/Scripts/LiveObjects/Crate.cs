using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableArea _interactableZone;
        [SerializeField] private PlayerClass _player;
        [SerializeField] private int _pieceToBreak = 5;

        private bool _isReadyToBreak = false;
        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            InteractableArea.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableArea zone)
        {
            
            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    StartCoroutine(PunchDelay());
                    PlayPlayerAnimation(zone);
                }
                else if(_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
            
        }

        private void PlayPlayerAnimation(InteractableArea zone)
        {

            if (InputManager.Instance.HoldPerformed())
            {
                _player.KickAnim();
                zone.ResetHoldAction(6);
                BreakParts();
            }
            else if (InputManager.Instance.TapPerformed())
            {
                _player.PuchAnim();
                zone.ResetTapAction(6);
                BreakPart();
            }
                
        }

        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);            
        }

        public void BreakParts()
        {
            if (_brakeOff.Count <= _pieceToBreak)
                _pieceToBreak = _brakeOff.Count;

            for (int i = 0; i < _pieceToBreak; i++)
            {
                int rng = Random.Range(0, _brakeOff.Count);

                _brakeOff[rng].constraints = RigidbodyConstraints.None;
                _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                _brakeOff.Remove(_brakeOff[rng]);
            }
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
            _interactableZone.ResetHoldAction(6);
            _interactableZone.ResetTapAction(6);
        }

        private void OnDisable()
        {
            InteractableArea.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
