using System;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    public class BobMovement
    {
        private readonly BobData _data;
        private readonly BodyMovementData _bodyData;
        private readonly float _maxPossibleSpeed;
        
        private float _bobFactor;
        private Vector3 _position;

        public BobMovement(BobData data, BodyMovementData bodyData)
        {
            _data = data;
            _bodyData = bodyData;
        }

        public Vector3 CalculateBob(Vector3 velocity, bool canBob, bool toggleBobAmount = false)
        {
            if (!(Time.deltaTime > 0f)) return Vector3.zero;

            var playerVel = velocity;
            playerVel.y = 0;
            
            var playerMovementFactor = 0f;
            if (canBob)
            {
                playerMovementFactor = Mathf.Clamp01(playerVel.magnitude / GetMaxPossibleSpeed());
            }
            
            _bobFactor = 
                Mathf.Lerp(_bobFactor, playerMovementFactor, _data.bobSharpness * Time.deltaTime);

            //Divided by 100 so can use bigger numbers in instructor.
            var bobAmount = (toggleBobAmount ? _data.alternativeBobAmount : _data.defaultBobAmount)/100; 
            var frequency = _data.bobFrequency;
            var hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _bobFactor;
            var vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                            _bobFactor;

            _position.x = hBobValue;
            _position.y = Mathf.Abs(vBobValue);
            return _position;
        }

        private float GetMaxPossibleSpeed()
        {
            return _bodyData.maxGroundSpeed * _bodyData.sprintSpeedModifier;
        }
    }

    [Serializable]
    public class BobData
    {
        [Range(1f,20f)] public float bobSharpness = 10f;
        [Range(1f,20f)] public float bobFrequency = 10f;
        [Range(1,10)] public float defaultBobAmount = 5f;
        [Range(1,10)] public float alternativeBobAmount = 2f;
    }
}