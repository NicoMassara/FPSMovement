﻿using System;
using UnityEngine;

namespace _Main.Scripts.Weapons
{
    public class BulletStandard : BulletBase
    {
        [SerializeField] private StandardBulletData data;
        [SerializeField] private float trajectoryCorrectionDistance = 5;
        [SerializeField] private bool inheritWeaponVelocity;
        [SerializeField] private Transform root;

        private Vector3 _lastRootPosition;
        private Vector3 _velocity;
        private Vector3 _trajectoryCorrectionVector;
        private Vector3 _consumedTrajectoryCorrectionVector;
        private bool _hasTrajectoryOverride;
        
        private void Awake()
        {
            base.OnShoot += OnShootHandler;
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;

            if (inheritWeaponVelocity)
            {
                transform.position += base.InheritedMuzzleVelocity * Time.deltaTime;
            }

            if (_hasTrajectoryOverride &&
                _consumedTrajectoryCorrectionVector.sqrMagnitude < _trajectoryCorrectionVector.sqrMagnitude)
            {
                var correctionLeft = _trajectoryCorrectionVector - _consumedTrajectoryCorrectionVector;
                var distanceThisFrame = (root.position - _lastRootPosition).magnitude;
                var correctionThisFrame =
                    (distanceThisFrame / trajectoryCorrectionDistance) * _trajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                _consumedTrajectoryCorrectionVector += correctionThisFrame;

                if (_consumedTrajectoryCorrectionVector.sqrMagnitude == _trajectoryCorrectionVector.sqrMagnitude)
                {
                    _hasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }

            transform.forward = _velocity.normalized;
            _lastRootPosition = root.position;
        }

        private void OnShootHandler()
        {
            _lastRootPosition = root.position;
            _velocity = base.SelfTransform.forward * data.speed;
            transform.position += base.InheritedMuzzleVelocity * Time.deltaTime;
            
            if (base.CameraTransform)
            {
                _hasTrajectoryOverride = true;
                
                var cameraToMuzzle = base.InitialPosition - CameraTransform.position;

                _trajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                    CameraTransform.forward);

                if (trajectoryCorrectionDistance == 0)
                {
                    transform.position += _trajectoryCorrectionVector;
                    _consumedTrajectoryCorrectionVector = _trajectoryCorrectionVector;
                }
                else if (trajectoryCorrectionDistance < 0)
                {
                    _hasTrajectoryOverride = false;
                }
            }
        }
    }

    [Serializable]
    public class StandardBulletData
    {
        public float speed;
        public float maxLifeTime;
    }
}