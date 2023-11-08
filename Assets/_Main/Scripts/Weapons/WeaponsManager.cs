using System;
using _Main.Scripts.Character.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Weapons
{
    public class WeaponsManager : MonoBehaviour
    {
        [Header("General Values")]
        [SerializeField] private Camera weaponCamera;
        [SerializeField] private PlayerComponentsData componentsData;
        [SerializeField] private Transform handsSocket;
        [SerializeField] private WeaponController currentWeapon;

        private BobMovement _bobMovement;
        private SwayMovement _swayMovement;
        private RecoilController _recoilController;

        private MovementController _movementController;
        
        private float _lastShootTime;
        private bool _isShooting;
        
        private Vector3 _muzzleVelocity;
        private Vector3 _lastMuzzlePosition;
        public Camera WeaponCamera => weaponCamera;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            
            _swayMovement = new SwayMovement(componentsData.swayData);
            _recoilController = new RecoilController(componentsData.recoilData);
            _bobMovement = new BobMovement(componentsData.bobData, componentsData.bodyData);
        }

        private void Start()
        {
            currentWeapon.OnShoot += OnShootHandler;
            currentWeapon.Owner = gameObject;
        }

        private void LateUpdate()
        {
            var isGrounded = _movementController.GetIsGrounded();
            var velocity = _movementController.GetVelocity();
            var mouseInput = _movementController.GetMouseInput();
            
            handsSocket.localRotation = Quaternion.Euler(_swayMovement.Calculate(mouseInput));
            handsSocket.localPosition = _bobMovement.CalculateBob(velocity,isGrounded) 
                                        + _recoilController.Calculate(_isShooting);
        }

        public void HandleShoot(bool isShooting)
        {
            if (isShooting)
            {
                _isShooting = currentWeapon.TryShoot(WeaponCamera.transform);
            }
        }

        private void OnShootHandler()
        {
            _movementController.ImpactCamera(Vector3.left);
        }
    }
}