using System;
using _Main.Scripts.Bullet;
using _Main.Scripts.Character.Components;
using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerModel : MonoBehaviour
    {
        [Header("General Values")] 
        [SerializeField] private PlayerComponentsData componentsData;
        [SerializeField] private Transform handsSocket;
        [SerializeField] private WeaponsManager weaponsManager;
        
        private BobMovement _bobMovement;
        private SwayMovement _swayMovement;
        private RecoilController _recoilController;
        private MovementController _movementController;

        private Vector3 _startPos;
        private Vector2 _mouseInput;
        
        //WeaponController
        private bool _isShooting;
        
        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            _swayMovement = new SwayMovement(componentsData.swayData);
            _recoilController = new RecoilController(componentsData.recoilData);
            _bobMovement = new BobMovement(componentsData.bobData, componentsData.bodyData);
        }

        private void Start()
        {
            _startPos = transform.position;
        }
        
        private void Update()
        {
            if (transform.position.y < -100f)
            {
                transform.position = _startPos;
            }
        }

        private void LateUpdate()
        {
            var isGrounded = _movementController.GetIsGrounded();
            var velocity = _movementController.GetVelocity();
            
            handsSocket.localRotation = Quaternion.Euler(_swayMovement.Calculate(_mouseInput));
            handsSocket.localPosition = _bobMovement.CalculateBob(velocity,isGrounded) 
                                        + _recoilController.Calculate(_isShooting);
        }

        public void Move(Vector2 input)
        {
            _movementController.Move(input);
        }
        
        public void Rotate(Vector2 mouseInput)
        {
            _mouseInput = mouseInput;
            _movementController.Rotate(_mouseInput);
        }

        public void Jump()
        {
            _movementController.Jump();
        }

        public void Sprint(bool input)
        {
            _movementController.SetSprint(input);
        }

        public void Shoot(bool isShooting)
        {
            _isShooting = isShooting;
            if (_isShooting)
            {
                weaponsManager.Shoot();
                _movementController.ImpactCamera(Vector3.down);
            }
        }

        public void UseJetpack(bool hasPressed, bool isPressed)
        {
            _movementController.UseJetpack(hasPressed,isPressed);
        }

    }
}