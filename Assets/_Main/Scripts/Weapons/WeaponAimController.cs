using System;
using UnityEngine;

namespace _Main.Scripts.Weapons
{
    public class WeaponAimController
    {
        private readonly WeaponAimData _data;

        private float _currentFov;
        private readonly Transform _aimLocation;
        private readonly Transform _defaultLocation;
        private Vector3 _mainLocation;

        public WeaponAimController(WeaponAimData data, Transform aimLocation, Transform defaultLocation)
        {
            _data = data;
            _aimLocation = aimLocation;
            _defaultLocation = defaultLocation;
        }

        public Vector3 CalculatePosition(bool isAiming, Vector3 aimOffset)
        {
            if (isAiming)
            {
                _mainLocation = Vector3.Lerp(_mainLocation, _aimLocation.localPosition + aimOffset,
                    _data.aimSpeed * Time.deltaTime);
            }
            else
            {
                _mainLocation = Vector3.Lerp(_mainLocation, _defaultLocation.localPosition, _data.aimSpeed*Time.deltaTime);
            }
            
            return _mainLocation;
        }
    }

    [Serializable]
    public class WeaponAimData
    {
        [Range(0, 30)] public float aimSpeed = 10f;
    }
}