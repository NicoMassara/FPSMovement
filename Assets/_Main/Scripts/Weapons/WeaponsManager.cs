using System;
using _Main.Scripts.Character.Components;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Weapons
{
    public class WeaponsManager : MonoBehaviour
    {
        [Header("General Values")]
        [SerializeField] private Camera weaponCamera;
        [SerializeField] private PlayerComponentsData componentsData;
        [SerializeField] private Transform handsSocket;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private WeaponController currentWeapon;
        [SerializeField] private WeaponController[] weaponSlots;
        [Space] 
        [Header("Position Values")] 
        [SerializeField] private Transform defaultPosition;
        [SerializeField] private Transform downPosition;
        [SerializeField] private Transform aimPosition;

        private BobMovement _bobMovement;
        private SwayMovement _swayMovement;
        private RecoilController _recoilController;
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

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            
            _swayMovement = new SwayMovement(componentsData.swayData);
            _recoilController = new RecoilController(componentsData.recoilData);
            _bobMovement = new BobMovement(componentsData.bobData, componentsData.bodyData);
            _switchController = new WeaponSwitchController(
                componentsData.weaponSwitchData, weaponSlots, defaultPosition, downPosition);
            _aimController = new WeaponAimController(componentsData.weaponAimData, aimPosition,defaultPosition);
            
            _hasActiveWeapon = _switchController.GetActiveWeapon();
        }

        private void Start()
        {
            currentWeapon.OnShoot += OnShootHandler;
            currentWeapon.Owner = gameObject;
            _switchController.OnSwitched += OnSwitchedHandler;
            _switchController.OnSwitched += OnWeaponSwitched;
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

            var aimMovement = _aimController.CalculatePosition(_isAiming, GetAimOffset());
            var bobMovement = _bobMovement.CalculateBob(velocity, isGrounded,_isAiming);
            var recoilMovement = _recoilController.Calculate(_isShooting);
            var switchMovement = _switchController.CalculateSwitchMovement();
            
            handsSocket.localRotation = Quaternion.Euler(_swayMovement.Calculate(mouseInput));
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
            
            return _isAiming;
        }
        
        public void HandleSwitch()
        {
            if (_isAiming) return;
            
            _switchController.SwitchWeapon(false);
        }

        private Vector3 GetAimOffset()
        {
            return currentWeapon ? currentWeapon.AimOffset : Vector3.zero;
        }
        
        public WeaponController GetCurrentWeapon()
        {
            return _hasActiveWeapon ? currentWeapon : default;
        }

        private void OnShootHandler()
        {
            _movementController.ImpactCamera(Vector3.left);
        }
        
        private void OnSwitchedHandler(WeaponController newWeapon)
        {
            currentWeapon.OnShoot -= OnShootHandler;
            _hasActiveWeapon = currentWeapon;
            currentWeapon = newWeapon;
            currentWeapon.OnShoot += OnShootHandler;
        }
    }
}