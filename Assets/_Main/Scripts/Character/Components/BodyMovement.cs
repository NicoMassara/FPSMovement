using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    public class BodyMovement
    {
        private readonly BodyMovementData _data;
        private readonly CharacterController _controller;
        private readonly Transform _player;
        private readonly float _gravity;

        private Vector3 _rotationAxis = Vector3.zero;
        private Vector3 _worldSpaceMoveInput = Vector3.zero;
        private bool _hasJumped;
        private float _velocityY;
        private readonly Transform _groundCheck;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsSprinting { get; private set; }
        public bool IsGrounded { get; private set; }

        public BodyMovement(BodyMovementData data, CharacterController controller, Transform transform, Transform groundCheck)
        {
            _data = data;
            _controller = controller;
            _player = transform;
            _controller.enableOverlapRecovery = true;
            _gravity = Physics.gravity.y;
            _groundCheck = groundCheck;
        }

        public void UpdateBody()
        {
            CheckGround();
            HandleMovement();
        }

        public void CheckGround()
        {
            IsGrounded = Physics.CheckSphere(_groundCheck.position, 0.2f, _data.whatIsGround);
        }

        public void HandleMovement()
        {
            _player.Rotate(_rotationAxis, Space.Self);
            
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
            
            _velocityY += _gravity * 2f * Time.deltaTime;
            targetVelocity.y = _velocityY;

            CharacterVelocity = targetVelocity;
            
            _controller.Move(CharacterVelocity * Time.deltaTime);

            if (IsGrounded! && CharacterVelocity.y < -1f)
            {
                _velocityY = -8f;
            }
        }

        public void Move(Vector2 input)
        {
            _worldSpaceMoveInput = _player.TransformVector(new Vector3(input.x, 0, input.y));
        }

        public void Rotate(float hAxis)
        {
            _rotationAxis.y = hAxis * _data.rotationSpeed/10;
        }

        public void Jump()
        {
            if(!IsGrounded) return;

            _velocityY += Mathf.Sqrt((_data.jumpForce * 10) * -2f * _gravity);
        }

        public void AddYAcceleration(float acceleration)
        {
            _velocityY += acceleration;
        }

        public void SetSprinting(bool isSprinting)
        {
            var flatVel = CharacterVelocity;
            flatVel.y = 0;
            
            if (flatVel.magnitude > 0.1f)
            {
                this.IsSprinting = isSprinting;
            }
            else if (flatVel.magnitude < 0.1f)
            {
                this.IsSprinting = false;
            }
        }

        public float GetRotationSpeed()
        {
            return _data.rotationSpeed;
        }
    }

    [Serializable]
    public class BodyMovementData
    {
        [Header("General")] 
        public LayerMask whatIsGround;
        [Header("Movement")]
        [Range(1f,100f)] public float maxGroundSpeed = 10f;
        [Range(1f,100f)] public float maxAirSpeed = 8f;
        [Range(1f,10f)] public float sprintSpeedModifier = 2f;
        [Header("Jump")]
        [Range(1f,50f)]public float jumpForce = 10f;
        [Space]
        [Header("Rotation")]
        [Range(1,100)]public float rotationSpeed = 20f;
    }
}