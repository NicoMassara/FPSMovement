using System;
using UnityEngine;

namespace _Main.Scripts.Jetpack
{
    public class JetpackController
    {
        private readonly JetpackData _data;
        private float _lastTimeOfUse;
        private float _gravityDownforce;
        public float CurrentFillRatio { get; private set; }

        public JetpackController(JetpackData data)
        {
            _data = data;
            _gravityDownforce = Physics.gravity.y;
            CurrentFillRatio = 1;
        }

        public Vector3 CalculateAcceleration(Vector3 bodyVelocity, bool isUsing)
        {
            var accelerationVector = Vector3.zero;

            if (isUsing)
            {
                _lastTimeOfUse = Time.time;
                
                float totalAcceleration = _data.acceleration + _gravityDownforce;

                if (bodyVelocity.y < 0f)
                {
                    totalAcceleration += (-bodyVelocity.y / Time.deltaTime) * _data.downwardVelocityCancelingFactor;
                }
                
                CurrentFillRatio -= Time.deltaTime / _data.consumeDuration;
                accelerationVector = Vector3.up * (totalAcceleration * Time.deltaTime);
            }
            else if(Time.time - _lastTimeOfUse >= _data.refillDelay)
            {
                var refillRate = 1 / _data.refillDuration;
                CurrentFillRatio += Time.deltaTime * refillRate;
            }
            
            CurrentFillRatio = Mathf.Clamp01(CurrentFillRatio);
            
            return accelerationVector;
        }

        public bool GetCanUse()
        {
            return CurrentFillRatio > 0;
        }
    }

    [Serializable]
    public class JetpackData
    {
        [Range(10,50)]public float acceleration = 25;
        [Range(1,10)]public float consumeDuration = 1.5f;
        [Range(0,15)]public float refillDuration = 2f;
        [Range(0,1)]public float downwardVelocityCancelingFactor = 1f;
        [Range(0,10)]public float refillDelay = 1f;
    }
}