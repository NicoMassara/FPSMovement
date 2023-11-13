using System;
using UnityEngine;

namespace _Main.Scripts.Weapons.Components
{
    public class WeaponBobMovement
    {
        private readonly WeaponBobData _data;
        private readonly float _maxPossibleSpeed;
        
        private float _bobFactor;
        private Vector3 _position;

        public WeaponBobMovement(WeaponBobData data)
        {
            _data = data;
        }

        public Vector3 CalculateBob(Vector3 velocity, float maxSpeed, bool canBob, bool isAiming = false)
        {
            if (!(Time.deltaTime > 0f)) return Vector3.zero;

            var playerVel = velocity;
            playerVel.y = 0;
            
            var playerMovementFactor = 0f;
            if (canBob)
            {
                playerMovementFactor = Mathf.Clamp01(playerVel.magnitude / maxSpeed);
            }
            
            _bobFactor = 
                Mathf.Lerp(_bobFactor, playerMovementFactor, _data.bobSharpness * Time.deltaTime);

            //Divided by 100 so can use bigger numbers in instructor.
            var bobAmount = (isAiming ? _data.alternativeBobAmount : _data.defaultBobAmount)/100; 
            var frequency = _data.bobFrequency;
            var hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _bobFactor;
            var vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                            _bobFactor;

            _position.x = hBobValue;
            _position.y = Mathf.Abs(vBobValue);
            return _position;
        }
    }

    [Serializable]
    public class WeaponBobData
    {
        [Range(1f,20f)] public float bobSharpness = 10f;
        [Range(1f,20f)] public float bobFrequency = 10f;
        [Range(1,10)] public float defaultBobAmount = 5f;
        [Range(1,10)] public float alternativeBobAmount = 2f;
    }
}