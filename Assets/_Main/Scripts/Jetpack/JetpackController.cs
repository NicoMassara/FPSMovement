using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Jetpack
{
    public class JetpackController
    {
        private readonly JetpackData _data;
        private float _lastTimeOfUse;
        private readonly float _gravityDownforce;
        private bool _wasUsing;
        private bool _isUsing;
        
        public float CurrentFillRatio { get; private set; }

        public UnityAction OnStarted;
        public UnityAction OnStopped;

        public JetpackController(JetpackData data)
        {
            _data = data;
            _gravityDownforce = Physics.gravity.y;
            CurrentFillRatio = 1;
        }

        public float CalculateAcceleration(Vector3 bodyVelocity, bool isUsing)
        {
            var accelerationVector = Vector3.zero;

            if (isUsing)
            {
                _lastTimeOfUse = Time.time;

                float totalAcceleration = _data.acceleration;
                
                //Cancel out gravity
                totalAcceleration += Physics.gravity.y * -1;

                if (bodyVelocity.y < 0f)
                {
                    totalAcceleration += (-bodyVelocity.y / Time.deltaTime) * 
                                         _data.downwardVelocityCancelingFactor;
                }
                
                accelerationVector = Vector3.up * (totalAcceleration * Time.deltaTime);
                
                CurrentFillRatio -= Time.deltaTime / _data.consumeDuration;
            }
            else if(GetCanRefill() && DoesNeedRefill())
            {
                var refillRate = 1 / _data.refillDuration;
                CurrentFillRatio += Time.deltaTime * refillRate;
            }
            
            CurrentFillRatio = Mathf.Clamp01(CurrentFillRatio);

            if (_wasUsing && !isUsing)
            {
                OnStopped?.Invoke();
            }
            else if (!_wasUsing && isUsing)
            {
                OnStarted?.Invoke();
            }

            _wasUsing = isUsing;
            return accelerationVector.y;
        }
        
        public bool GetCanUse()
        {
            return CurrentFillRatio > 0;
        }

        private bool GetCanRefill()
        {
            return Time.time - _lastTimeOfUse >= _data.refillDelay;
        }

        private bool DoesNeedRefill()
        {
            return CurrentFillRatio < 1;
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