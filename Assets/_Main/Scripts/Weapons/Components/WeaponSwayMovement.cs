using System;
using UnityEngine;

namespace _Main.Scripts.Weapons.Components
{
    public class WeaponSwayMovement
    {
        private readonly WeaponSwayData _data;
        private Vector3 _targetRotation;
        private Vector3 _newRotation;
        private Vector3 _targetVelocity;
        private Vector3 _newVelocity;
        private float _clampX;
        private float _clampY;

        public WeaponSwayMovement(WeaponSwayData data)
        {
            _data = data;
        }

        public Vector3 Calculate(Vector2 input)
        {
            _targetRotation.y += _data.amount * input.x * Time.deltaTime;
            _targetRotation.x += _data.amount * input.y * Time.deltaTime;
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, -_data.clampX, _data.clampX);
            _targetRotation.y = Mathf.Clamp(_targetRotation.y, -_data.clampY, _data.clampY);

            _targetRotation = Vector3.SmoothDamp(_targetRotation, Vector3.zero, ref _targetVelocity, _data.resetSmoothing/100);
            _newRotation = Vector3.SmoothDamp(_newRotation,_targetRotation, ref _newVelocity, _data.smoothDamp/100);
            
            return _newRotation;
        }
    }

    [Serializable]
    public class WeaponSwayData
    {
        [Range(1,20)]public float amount = 10;
        [Range(1,10)]public float smoothDamp = 1;
        [Range(1,10)]public float resetSmoothing = 1;
        [Range(1,10)]public float clampX = 1;
        [Range(1,10)]public float clampY = 1;
    }
}