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
        private readonly Transform _player;
        private readonly float _gravity;

        private readonly Transform _groundCheck;
        private float _rotationAngle;
        private Vector3 _worldSpaceMoveInput = Vector3.zero;
        private bool _hasJumped;
        private float _verticalVelocity;
        private bool _wasSprinting;
        private bool _wasOnGround;
        private const float CanSprintAgainDelay = 0.5f;
        private float _lastSprintTime;
        private const float CanJumpAgainDelay = 0.5f;
        private float _lastGroundedTime;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsSprinting { get; private set; }
        public bool IsGrounded { get; private set; }

        public UnityAction OnSprint;
        public UnityAction OnLand;

        public BodyMovement(BodyMovementData data, CharacterController controller, Transform transform, Transform groundCheck)
        {
            _data = data;
            _controller = controller;
            _player = transform;
            _controller.enableOverlapRecovery = true;
            _gravity = Physics.gravity.y;
            _groundCheck = groundCheck;
            _wasOnGround = true;
        }

        public void UpdateBody()
        {
            CheckGround();
            HandleMovement();
            
            if (!_wasOnGround && IsGrounded)
            {
                _lastGroundedTime = Time.time;
                OnLand?.Invoke();
            }

            _wasOnGround = IsGrounded;
        }

        private void CheckGround()
        {
            IsGrounded = Physics.CheckSphere(_groundCheck.position, 0.2f, _data.whatIsGround);
        }

        private void HandleMovement()
        {
            _player.Rotate(_rotationAngle * Vector3.up, Space.Self);
            
            var speedModifier = IsSprinting ? _data.sprintSpeedModifier : 1f;

            Vector3 targetVelocity;
            
            if (IsGrounded)
            {
                targetVelocity = _worldSpaceMoveInput * (_data.maxGroundSpeed * speedModifier);
            }
            else
            {
                targetVelocity = _worldSpaceMoveInput * (_data.maxAirSpeed);
            }
            
            _verticalVelocity += _gravity * 2f * Time.deltaTime;
            targetVelocity.y = _verticalVelocity;

            CharacterVelocity = targetVelocity;
            
            _controller.Move(CharacterVelocity * Time.deltaTime);

            if (IsGrounded! && CharacterVelocity.y < -1f)
            {
                _verticalVelocity = -8f;
            }
        }

        public void Move(Vector2 input)
        {
            _worldSpaceMoveInput = _player.TransformVector(new Vector3(input.x, 0, input.y));
        }

        public void Rotate(float hAxis)
        {
            _rotationAngle = hAxis;
        }

        public void Jump()
        {
            if (IsGrounded && GetCanJumpAgain())
            {
                _verticalVelocity += Mathf.Sqrt((_data.jumpForce * 10) * -2f * _gravity);
            }
        }

        public void AddYAcceleration(float acceleration)
        {
            _verticalVelocity += acceleration;
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

        public float GetMaxPossibleSpeed()
        {
            return (IsGrounded ? _data.maxGroundSpeed : _data.maxAirSpeed) * _data.sprintSpeedModifier;
        }
        
        private bool GetCanSprintAgain()
        {
            return Time.time - _lastSprintTime >= CanSprintAgainDelay;
        }

        private bool GetCanJumpAgain()
        {
            return Time.time - _lastGroundedTime >= CanJumpAgainDelay;
        }
    }

    [Serializable]
    public class BodyMovementData
    {
        [Header("General")] 
        public LayerMask whatIsGround;
        [Header("Movement")]
        [Range(1f,20f)] public float maxGroundSpeed = 10f;
        [Range(1f,20f)] public float maxAirSpeed = 8f;
        [Range(1f,5f)] public float sprintSpeedModifier = 2f;
        [Header("Jump")]
        [Range(1f,10f)]public float jumpForce = 10f;
    }
}