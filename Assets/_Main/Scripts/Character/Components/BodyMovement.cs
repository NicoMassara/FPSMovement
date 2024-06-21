using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Character.Components
{
    public class BodyMovement
    {
        private readonly BodyMovementData _data;
        private readonly CharacterController _controller;
        private readonly Transform _transform;
        private readonly float _gravity;

        private const float CanSprintAgainDelay = 0.5f;
        private const float FootstepFrequency = 0.3f;
        private const float FootstepFrequencySprinting = 0.2f;
        private float _rotationAngle;
        private float _verticalVelocity;
        private float _lastSprintTime, _lastTimeJumped;
        private float _footStepsDistanceCounter;
        private bool _wasGrounded ,_wasSprinting;
        private bool _hasPressedJumpInput;
        private Vector3 _worldSpaceMoveInput = Vector3.zero;
        private Vector3 _groundNormal;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsSprinting { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }

        public UnityAction OnSprint, OnLand, OnJump, OnWalkStep;
        
        public BodyMovement(BodyMovementData data, CharacterController controller, Transform transform)
        {
            _data = data;
            _controller = controller;
            _transform = transform;
            _controller.enableOverlapRecovery = true;
            _gravity = Physics.gravity.y*-1;
            _wasGrounded = true;
        }

        public void UpdateBody()
        {
            HasJumpedThisFrame = false;
            _wasGrounded = IsGrounded;
            
            CheckGround();
            
            //Handle landing
            if (IsGrounded && !_wasGrounded)
            {
                OnLand?.Invoke();
            }
            
            UpdateMovement();
            
        }
        
        #region Movement

        private void UpdateMovement()
        {
            _transform.Rotate(_rotationAngle * Vector3.up, Space.Self);
            
            var speedModifier = IsSprinting ? _data.sprintSpeedModifier : 1f;
            
            if (IsGrounded)
            {
                HandleGroundMovement(speedModifier);
                HandleJump();
                HandleFootsteps();
            }
            else
            {
                HandleAirMovement(speedModifier);
            }
            
            _controller.Move(CharacterVelocity * Time.deltaTime);
        }
        
        private void HandleGroundMovement(float speedModifier)
        {
            //Calculate desired speed
            Vector3 targetVelocity = _worldSpaceMoveInput * (_data.maxGroundSpeed * speedModifier);

            //Smoothly interpolate velocity from current to desired
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                _data.movementSharpnessOnGround * Time.deltaTime);
        }
        
        private void HandleJump()
        {
            if (IsGrounded && _hasPressedJumpInput)
            {
                _hasPressedJumpInput = false;

                //Cancel vertical velocity
                CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                // Add vertical velocity
                CharacterVelocity += Vector3.up * _data.jumpForce;

                OnJump?.Invoke();
                _lastTimeJumped = Time.time;
                HasJumpedThisFrame = true;

                IsGrounded = false;
                _groundNormal = Vector3.up;
            }
        }

        private void HandleAirMovement(float speedModifier)
        {
            //Add air acceleration
            CharacterVelocity += _worldSpaceMoveInput * (_data.accelerationSpeedInAir * Time.deltaTime);
            
            //Limit air speed, horizontally
            _verticalVelocity = CharacterVelocity.y;
            var horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _data.maxAirSpeed * speedModifier);
            CharacterVelocity = horizontalVelocity + (Vector3.up * _verticalVelocity);
            
            //Apply downforce
            CharacterVelocity += Vector3.down * (_gravity * Time.deltaTime);
        }
        
        private void HandleFootsteps()
        {
            float chosenFrequency =
                IsSprinting ? FootstepFrequencySprinting : FootstepFrequency;

            if (_footStepsDistanceCounter >= 1f / chosenFrequency)
            {
                _footStepsDistanceCounter = 0f;
                OnWalkStep?.Invoke();
            }
            
            //Keep track of traveled distance
            _footStepsDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
        }

        public void Move(Vector2 input)
        {
            _worldSpaceMoveInput = _transform.TransformVector(new Vector3(input.x, 0, input.y));
        }

        public void Rotate(float hAxis)
        {
            _rotationAngle = hAxis;
        }

        public void Jump()
        {
            if (IsGrounded)
            {
                _hasPressedJumpInput = true;
            }
        }
        
        public void AddVerticalAcceleration(float acceleration)
        {
            CharacterVelocity += Vector3.up * acceleration;
        }

        public void SetSprinting(bool isSprinting)
        {
            var flatVel = CharacterVelocity;
            flatVel.y = 0;
            
            _wasSprinting = IsSprinting;
            
            
            if (flatVel.magnitude > 0.1f && GetCanSprintAgain())
            {
                this.IsSprinting = isSprinting;
            }
            else if (flatVel.magnitude < 0.1f)
            {
                this.IsSprinting = false;
            }
            
            if (!_wasSprinting && IsSprinting)
            {
                OnSprint?.Invoke();
                _lastSprintTime = Time.time;
            }
        }

        #endregion

        #region Ground Check
        
        private void CheckGround()
        {
            float groundCheckDistance = 
                IsGrounded ? (_controller.skinWidth + _data.groundCheckDistance) : _data.groundCheckDistanceInAir;
            
            //Reset values before check
            IsGrounded = false;
            _groundNormal = Vector3.up;

            //Only try to detect after a short amount of time. Prevents instant snap to the ground after jump
            if (Time.time >= _lastTimeJumped + _data.jumpGroundingPreventionTime)
            {
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(_controller.height),
                        _controller.radius, Vector3.down, out var hit, groundCheckDistance, _data.whatIsGround,
                        QueryTriggerInteraction.Ignore))
                {
                    _groundNormal = hit.normal;

                    if (Vector3.Dot(hit.normal, _transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(_groundNormal))
                    {
                        
                        IsGrounded = true;
                        
                        //Handle snapping to the ground
                        if (hit.distance > _controller.skinWidth)
                        {
                            _controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }
        
        // Gets the center point of the bottom hemisphere of the character controller capsule    
        private Vector3 GetCapsuleBottomHemisphere()
        {
            return _transform.position + (_transform.up * _controller.radius);
        }
        
        // Gets the center point of the top hemisphere of the character controller capsule  
        private Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return _transform.position + (_transform.up * (atHeight - _controller.radius));
        }
        
        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        private bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(_transform.up, normal) <= _controller.slopeLimit;
        }
        
        // Gets a reoriented direction that is tangent to a given slope
        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, _transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }
        
        
        #endregion
        
        #region Getters

        public float GetMaxPossibleSpeed()
        {
            return (IsGrounded ? _data.maxGroundSpeed : _data.maxAirSpeed) * _data.sprintSpeedModifier;
        }
        
        private bool GetCanSprintAgain()
        {
            return Time.time - _lastSprintTime >= CanSprintAgainDelay;
        }

        #endregion
    }

    [Serializable]
    public class BodyMovementData
    {
        [Header("General")] 
        public LayerMask whatIsGround;
        public float groundCheckDistance = 1f;
        public float groundCheckDistanceInAir = 0.07f;
        [Header("Ground Movement")]
        [Range(1f,20f)] public float maxGroundSpeed = 10f;
        [Range(1f,5f)] public float sprintSpeedModifier = 2f;
        public float movementSharpnessOnGround = 15f;
        [Header("Air Movement")]
        [Range(1f,20f)] public float maxAirSpeed = 8f;
        public float accelerationSpeedInAir = 25f;
        [Header("Jump")]
        [Range(1f,10f)]public float jumpForce = 10f;
        public float jumpGroundingPreventionTime = 0.2f;
    }
}