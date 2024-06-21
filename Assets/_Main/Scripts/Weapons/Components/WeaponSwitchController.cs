using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Weapons.Components
{
    public class WeaponSwitchController
    {
        private readonly float _switchDelay;
        private float _timeStartedWeaponSwitch;
        private readonly Transform _defaultLocalPosition;
        private readonly Transform _downLocalPosition;
        private readonly WeaponController[] _slots;

        private int _newWeaponIndex;
        private int _activeWeaponIndex;
        private Vector3 _mainLocalPosition;
        private WeaponSwitchState _currentState;
        
        public UnityAction<WeaponController> OnSwitched;
        
        private enum WeaponSwitchState
        {
            Up,
            Down,
            PutDownPrevious,
            PutUpNew,
        }

        public WeaponSwitchController(float switchDelay, WeaponController[] slots, Transform defaultLocalPosition,
            Transform downLocalPosition)
        {
            _switchDelay = switchDelay;
            _defaultLocalPosition = defaultLocalPosition;
            _downLocalPosition = downLocalPosition;
            _slots = slots;
            _currentState = WeaponSwitchState.Up;
            
            OnSwitched += OnSwitchedHandler;
        }

        public void SwitchWeapon(bool isBackwards)
        {
            if (_currentState is not (WeaponSwitchState.Up or WeaponSwitchState.Down)) return;
            
            int newIndex = -1;
            int closestSlotDistance = _slots.Length;

            for (int i = 0; i < _slots.Length; i++)
            {
                if (i != _activeWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(_activeWeaponIndex, i, isBackwards);

                    if (distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newIndex = i;
                    }
                }
            }

            SwitchToWeaponIndex(newIndex);
        }
        
        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(_activeWeaponIndex);
        }

        public bool GetCanUse()
        {
            return _currentState is WeaponSwitchState.Up;
        }

        public Vector3 CalculateSwitchMovement()
        {
            float switchingTimeFactor = 0f;
            
            if (_switchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - _timeStartedWeaponSwitch) / _switchDelay);
            }

            if (switchingTimeFactor >= 1f)
            {
                if (_currentState == WeaponSwitchState.PutDownPrevious)
                {
                    var oldWeapon = GetWeaponAtSlotIndex(_activeWeaponIndex);
                    oldWeapon.ShowWeapon(false);

                    _activeWeaponIndex = _newWeaponIndex;
                    switchingTimeFactor = 0f;

                    var newWeapon = GetWeaponAtSlotIndex(_activeWeaponIndex);
                    OnSwitched?.Invoke(newWeapon);
                    
                    if (newWeapon)
                    {
                        _timeStartedWeaponSwitch = Time.time;
                        _currentState = WeaponSwitchState.PutUpNew;
                    }
                    else
                    {
                        _currentState = WeaponSwitchState.Down;
                    }
                }
                else if (_currentState == WeaponSwitchState.PutUpNew)
                {
                    _currentState = WeaponSwitchState.Up;
                }
            }
            
            if (_currentState == WeaponSwitchState.PutDownPrevious)
            {
                _mainLocalPosition = Vector3.Lerp(
                    _defaultLocalPosition.localPosition,_downLocalPosition.localPosition,switchingTimeFactor);
            }
            else if(_currentState == WeaponSwitchState.PutUpNew)
            {
                _mainLocalPosition = Vector3.Lerp(
                    _downLocalPosition.localPosition, _defaultLocalPosition.localPosition, switchingTimeFactor);
            }

            return _mainLocalPosition;
        }
        
        private void SwitchToWeaponIndex(int newWeaponIndex)
        {
            if (newWeaponIndex != _activeWeaponIndex && newWeaponIndex >= 0)
            {
                _newWeaponIndex = newWeaponIndex;
                _timeStartedWeaponSwitch = Time.time;

                if (GetActiveWeapon() == null)
                {
                    _mainLocalPosition = _downLocalPosition.localPosition;
                    _currentState = WeaponSwitchState.PutUpNew;
                    _activeWeaponIndex = _newWeaponIndex;

                    var newWeapon = GetWeaponAtSlotIndex(_newWeaponIndex);
                    OnSwitched?.Invoke(newWeapon);
                }
                else
                {
                    _currentState = WeaponSwitchState.PutDownPrevious;
                }
            }
        }

        private WeaponController GetWeaponAtSlotIndex(int index)
        {
            // find the active weapon in our weapon slots based on our active weapon index
            if (index >= 0 &&
                index < _slots.Length)
            {
                return _slots[index];
            }

            // if we didn't find a valid active weapon in our weapon slots, return null
            return null;
        }

        private int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if (ascendingOrder)
            {
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            }
            else
            {
                distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
            }

            if (distanceBetweenSlots < 0)
            {
                distanceBetweenSlots = _slots.Length + distanceBetweenSlots;
            }

            return distanceBetweenSlots;
        }
        
        private void OnSwitchedHandler(WeaponController newWeapon)
        {
            newWeapon?.ShowWeapon(true);
        }
    }
}