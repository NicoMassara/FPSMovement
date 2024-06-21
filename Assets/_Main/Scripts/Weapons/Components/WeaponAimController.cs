using System;
using UnityEngine;

namespace _Main.Scripts.Weapons.Components
{
    public class WeaponAimController
    {
        private readonly float _aimSpeed;

        private float _currentFov;
        private readonly Transform _aimLocation;
        private readonly Transform _defaultLocation;
        private Vector3 _mainLocation;

        public WeaponAimController(float aimSpeed, Transform aimLocation, Transform defaultLocation)
        {
            _aimSpeed = aimSpeed;
            _aimLocation = aimLocation;
            _defaultLocation = defaultLocation;
        }

        public Vector3 CalculatePosition(bool isAiming, Vector3 aimOffset)
        {
            if (isAiming)
            {
                _mainLocation = Vector3.Lerp(_mainLocation, _aimLocation.localPosition + aimOffset,
                    _aimSpeed * Time.deltaTime);
            }
            else
            {
                _mainLocation = Vector3.Lerp(_mainLocation, _defaultLocation.localPosition, _aimSpeed*Time.deltaTime);
            }
            
            return _mainLocation;
        }
    }
}