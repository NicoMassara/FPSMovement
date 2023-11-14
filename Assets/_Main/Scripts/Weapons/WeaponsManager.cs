using System;
using _Main.Scripts.Character;
using _Main.Scripts.Sounds;
using _Main.Scripts.Weapons.Components;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Weapons
{
    public class WeaponsManager : MonoBehaviour
    {
        [Header("General Values")] 
        [SerializeField] private SoundClassSo weaponSwitchSound;
        [SerializeField] private Camera weaponCamera;
        [SerializeField] private WeaponHandsDataSo handsData;
        [SerializeField] private Transform handsSocket;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private WeaponController currentWeapon;
        [SerializeField] private WeaponController[] weaponSlots;
        [Space] 
        [Header("Position Values")] 
        [SerializeField] private Transform defaultPosition;
        [SerializeField] private Transform downPosition;
        [SerializeField] private Transform aimPosition;

        private WeaponBobMovement _weaponBobMovement;
        private WeaponSwayMovement _weaponSwayMovement;
        private WeaponSwitchController _switchController;
        private WeaponAimController _aimController;

        private MovementController _movementController;
        
        private float _lastShootTime;
        private bool _isShooting;
        private bool _isAiming;
        private bool _hasActiveWeapon;
        
        private Vector3 _muzzleVelocity;
        private Vector3 _lastMuzzlePosition;
        public Camera WeaponCamera => weaponCamera;
        public bool IsPointingAtEnemy { get; private set; }

        public UnityAction<WeaponController> OnWeaponSwitched;
        public UnityAction<bool> OnChangeAim;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            
            _weaponSwayMovement = new WeaponSwayMovement(handsData.SwayData);
            _weaponBobMovement = new WeaponBobMovement(handsData.BobData);
            _switchController = new WeaponSwitchController(handsData.SwitchDelay, 
                weaponSlots, defaultPosition, downPosition);
            _aimController = new WeaponAimController(handsData.AimSpeed, aimPosition,defaultPosition);
            
            _hasActiveWeapon = _switchController.GetActiveWeapon();
        }

        private void Start()
        {
            currentWeapon.OnShoot += OnShootHandler;
            currentWeapon.Owner = gameObject;
            _switchController.OnSwitched += OnSwitchedHandler;
        }

        private void Update()
        {
            IsPointingAtEnemy = false;
            
            if (_hasActiveWeapon)
            {
                if (Physics.Raycast(WeaponCamera.transform.position, WeaponCamera.transform.forward,
                        out var hit, 1000, whatIsEnemy))
                {
                    IsPointingAtEnemy = true;
                }
            }
        }

        private void LateUpdate()
        {
            var isGrounded = _movementController.GetIsGrounded();
            var velocity = _movementController.GetVelocity();
            var mouseInput = _movementController.GetMouseInput();
            var maxSpeed = _movementController.GetMaxPossibleSpeed();

            var aimMovement = _aimController.CalculatePosition(_isAiming, currentWeapon.GetAimOffset());
            var bobMovement = _weaponBobMovement.CalculateBob(velocity,maxSpeed, isGrounded,_isAiming);
            var recoilMovement = currentWeapon.GetRecoilMovement(_isShooting);
            var switchMovement = _switchController.CalculateSwitchMovement();
            
            handsSocket.localRotation = Quaternion.Euler(_weaponSwayMovement.Calculate(mouseInput));
            handsSocket.localPosition = bobMovement + recoilMovement + switchMovement + aimMovement;
        }
        

        public void HandleShoot(bool isShooting)
        {
            if (isShooting && _switchController.GetCanUse() && _hasActiveWeapon)
            {
                _isShooting = currentWeapon.TryShoot(WeaponCamera.transform);
            }
        }
        
        public bool TryAim(bool isAiming)
        {
            if (!_hasActiveWeapon) return false;
            if (!_switchController.GetCanUse()) return false;

            _isAiming = isAiming;
            OnChangeAim?.Invoke(_isAiming);
            
            return _isAiming;
        }
        
        public void HandleSwitch()
        {
            if (_isAiming) return;
            
            _switchController.SwitchWeapon(false);
            SoundManager.Singleton.PlaySoundAtLocation(weaponSwitchSound, transform.position);
        }
        
        public WeaponController GetCurrentWeapon()
        {
            return _hasActiveWeapon ? currentWeapon : default;
        }

        private void OnShootHandler()
        {
            _movementController.ImpactCamera(Vector3.left, currentWeapon.GetRecoilCameraForce());
        }
        
        private void OnSwitchedHandler(WeaponController newWeapon)
        {
            currentWeapon.OnShoot -= OnShootHandler;
            _hasActiveWeapon = currentWeapon;
            currentWeapon = newWeapon;
            currentWeapon.OnShoot += OnShootHandler;
            OnWeaponSwitched?.Invoke(currentWeapon);
        }
    }
}