using System;
using _Main.Scripts.Character.Components;
using _Main.Scripts.Jetpack;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private SoundClassSo sprintSound;
        [SerializeField] private SoundClassSo landSound;
        [SerializeField] private SoundClassSo jetpackSound;
        [SerializeField] private PlayerComponentsDataSo componentsData;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera weaponCamera;
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

        public PlayerComponentsDataSo ComponentsData => componentsData;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            _bodyMovement = new BodyMovement(componentsData.BodyData, _characterController,transform,groundCheck);
            _camera = new CameraMovement(componentsData.CameraData, mainCamera, weaponCamera);
            _jetpack = new JetpackController(componentsData.JetpackData);

            _bodyMovement.OnSprint += OnSprintHandler;
            _bodyMovement.OnLand = OnLandHandler;

            _jetpack.OnStarted += Jetpack_OnStartedHandler;
            _jetpack.OnStopped += Jetpack_OnStoppedHandler;
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
                ImpactCamera(Vector3.right * 
                             componentsData.CameraForceAtGroundImpact);
            }
            
            _lastFrameGrounded = GetIsGrounded();
            _lasFrameUsingJetpack = _isUsingJetpack;
        }

        private void LateUpdateCamera()
        {
            SetFov(FovType.Sprint, GetIsSprinting());
            _camera.UpdateAngle();
            _camera.Rotate(_mouseInput.y);
            _bodyMovement.Rotate(_mouseInput.x);
        }

        #region Get Values

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

        public bool GetIsMoving()
        {
            var flatVel = GetVelocity();
            flatVel.y = 0;
            return flatVel.magnitude > 0.1f;
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

        public float GetMaxPossibleSpeed()
        {
            return _bodyMovement.GetMaxPossibleSpeed();
        }

        #endregion

        #region Actions

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
            if (!GetIsGrounded()) return;
            
            _bodyMovement.SetSprinting(isSprinting);
        }

        public void Jump()
        {
            _bodyMovement.Jump();
        }

        public void Look(Vector2 mouseInput)
        {
            _mouseInput = mouseInput;
        }

        #endregion

        #region Camera Modifiers

        public void ImpactCamera(Vector3 direction, float multiplier = 1)
        {
            _camera.Impact(direction * multiplier);
        }

        public void SetFov(FovType fovType, bool input)
        {
            _camera.SetFov(fovType, input);
        }

        #endregion
        
        private void OnSprintHandler()
        {
            SoundManager.Singleton.PlaySoundAtLocation(sprintSound,transform.position);
        }

        private void OnLandHandler()
        {
            SoundManager.Singleton.PlaySoundAtLocation(landSound,transform.position);
        }
        private void Jetpack_OnStartedHandler()
        {
            SoundManager.Singleton.PlayStoppableSoundAtLocation(jetpackSound,transform);
        }
        
        private void Jetpack_OnStoppedHandler()
        {
            SoundManager.Singleton.StopSound();
        }
    }
}