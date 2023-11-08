using System;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    public class CameraMovement
    {
        private readonly CameraMovementData _data;
        private readonly Transform _cameraTransform;
        private readonly Camera _camera;
        private float _verticalAngle;
        private float _moveAngle;

        private const float TimeBeforeResetting = 0f;
        private float _resetTime;

        private Vector3 _impactAngle;

        public CameraMovement(CameraMovementData data, Camera camera)
        {
            _data = data;
            _camera = camera;
            _cameraTransform = _camera.transform;
        }

        public void Rotate(float vAxis, float rotationSpeed)
        {
            _verticalAngle += -vAxis * rotationSpeed;
            _verticalAngle = Mathf.Clamp(_verticalAngle, -89f, 89);
            _cameraTransform.transform.localEulerAngles = _impactAngle + (Vector3.right * _verticalAngle);
        }

        public void SetFov(bool change)
        {
            var currentFov = _camera.fieldOfView;

            var targetFov = change ? _data.sprintFov : _data.startFov;
            var transitionMultiplier = change ? 1 : 0.5f;
            currentFov = Mathf.Lerp(currentFov, targetFov, _data.transitionTime * transitionMultiplier * Time.deltaTime);

            _camera.fieldOfView = currentFov;
        }

        public void UpdateAngle()
        {
            if (_impactAngle.magnitude > 0 && _resetTime <= 0)
            {
                _impactAngle = Vector2.Lerp(_impactAngle, Vector2.zero, _data.restitutionTime * Time.deltaTime);
            }
            
            _resetTime -= Time.deltaTime;
        }

        public void Impact(Vector3 moveDir)
        {
            _impactAngle = moveDir * _data.moveAngle;
            _resetTime = TimeBeforeResetting;
        }
    }

    [Serializable]
    public class CameraMovementData
    {
        [Range(40,110)]public float startFov = 60f;
        [Range(60,130)]public float sprintFov = 70f;
        [Range(1, 35)] public float moveAngle = 15f;
        [Range(1,50)] public float transitionTime = 15f;
        [Range(1,50)] public float restitutionTime = 25f;
    }
};