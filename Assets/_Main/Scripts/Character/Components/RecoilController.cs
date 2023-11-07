using System;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    public class RecoilController
    {
        private readonly RecoilData _data;
        private Vector3 _accumulated;
        private Vector3 _localPosition;

        public RecoilController(RecoilData data)
        {
            _data = data;
        }
        
        public Vector3 Calculate(bool isShooting)
        {
            if (isShooting)
            {
                _accumulated += Vector3.back * ((_data.force/10) + 1);
                _accumulated = Vector3.ClampMagnitude(_accumulated, _data.maxDistance * 10);

                _localPosition = Vector3.Lerp(_localPosition, _accumulated, _data.sharpness * Time.deltaTime);
            }
            else
            {
                _localPosition = Vector3.Lerp(_localPosition, Vector3.zero,
                    _data.restitutionSharpness * Time.deltaTime);
                _accumulated = _localPosition;
            }
            
            return _localPosition;
        }
    }

    [Serializable]
    public class RecoilData
    {
        [Range(0,10)]public float force = 1;
        [Range(1,10f)]public float maxDistance = 5f;
        [Range(10,100)]public float sharpness = 50f;
        [Range(1,50)]public float restitutionSharpness = 10f;
    }
}