using System;
using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private PlayerComponentsData componentsData;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform groundCheck;
        
        
        //Movement
        private BodyMovement _bodyMovement;
        private CharacterController _characterController;
        private bool _lastFrameGrounded;
        
        //Camera
        private CameraMovement _camera;
        private Vector2 _mouseInput;
        
        //Jetpack
        private JetpackController _jetpack;
        private float _jetpackAcceleration;
        private bool _isUsingJetpack;
        private bool _canUseJetpack;
        private bool _lasFrameUsingJetpack;


        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            _bodyMovement = new BodyMovement(componentsData.bodyData, _characterController,transform,groundCheck);
            _camera = new CameraMovement(componentsData.cameraData, mainCamera);
            _jetpack = new JetpackController(componentsData.jetpackData);
        }

        private void Update()
        {
            UpdateMovement();
        }

        private void LateUpdate()
        {
            LateUpdateCamera();
        }


        private void UpdateMovement()
        {
            _bodyMovement.CheckGround();
            _bodyMovement.HandleMovement();
            _jetpackAcceleration = _jetpack.CalculateAcceleration
                (_bodyMovement.CharacterVelocity,_isUsingJetpack).y;
            _bodyMovement.AddYAcceleration(_jetpackAcceleration);
            
            if (GetIsGrounded() && !_lastFrameGrounded)
            {
                ImpactCamera(Vector3.right);
            }
            
            _lastFrameGrounded = GetIsGrounded();
            _lasFrameUsingJetpack = _isUsingJetpack;
        }

        private void LateUpdateCamera()
        {
            _camera.SetFov(GetIsSprinting());
            _camera.UpdateAngle();
            _camera.Rotate(_mouseInput.y, _bodyMovement.GetRotationSpeed()/10);
            _bodyMovement.Rotate(_mouseInput.x);
        }

        public bool GetIsGrounded()
        {
            return _bodyMovement?.IsGrounded ?? true;
        }

        public bool GetIsSprinting()
        {
            return _bodyMovement?.IsSprinting ?? false;
        }

        public Vector3 GetVelocity()
        {
            return _bodyMovement?.CharacterVelocity ?? Vector3.zero;
        }

        public float GetJetpackFuel()
        {
            return _jetpack?.CurrentFillRatio ?? 0;
        }

        public bool GetCanUseJetpack()
        {
            return _jetpack?.GetCanUse() ?? false;
        }

        public Vector2 GetMouseInput()
        {
            return _mouseInput;
        }

        public void ImpactCamera(Vector3 direction)
        {
            _camera.Impact(direction);
        }

        public void UseJetpack(bool hasPressed, bool isPressed)
        {
            if (GetIsGrounded())
            {
                _canUseJetpack = false;
            }
            else if(hasPressed)
            {
                _canUseJetpack = true;
            }
            
            _isUsingJetpack = _canUseJetpack && isPressed && GetCanUseJetpack();

            if (_isUsingJetpack && !_lasFrameUsingJetpack)
            {
                ImpactCamera(Vector3.right);
            }
        }
        
        public void Move(Vector2 input)
        {
            _bodyMovement.Move(input);
        }

        public void SetSprint(bool isSprinting)
        {
            _bodyMovement.SetSprinting(isSprinting);
        }

        public void Jump()
        {
            _bodyMovement.Jump();
        }

        public void Rotate(Vector2 mouseInput)
        {
            _mouseInput = mouseInput;
        }
    }
}