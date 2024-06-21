using System;
using _Main.Scripts.Sounds;
using _Main.Scripts.Weapons.Components;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [Header("General Values")]
        [SerializeField] private WeaponDataSo weaponData;
        [SerializeField] private Transform muzzle;
        [SerializeField] private BulletStandard bulletPrefab;
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject muzzleFlashPrefab;

        private bool _hasMuzzleFlash;
        
        private WeaponShootData _shootData;
        private WeaponRecoilController _recoilController;
        
        private float _lastShootTime;
        private bool _isShooting;
        
        private Vector3 _muzzleVelocity;
        private Vector3 _lastMuzzlePosition;

        private SoundManager _soundManager;
        
        public bool IsWeaponActive { get; private set; }
        public GameObject Owner { get; set; }
        public WeaponDataSo WeaponData => weaponData;
        
        public UnityAction OnShoot;

        private void Awake()
        {
            _shootData = WeaponData.ShootData;
            OnShoot += OnShootHandler;
            _recoilController = new WeaponRecoilController(WeaponData.RecoilData);

            _soundManager = SoundManager.Singleton;
            _hasMuzzleFlash = muzzleFlashPrefab;
        }
        

        private void Update()
        {
            if (Time.deltaTime > 0)
            {
                var position = muzzle.position;
                _muzzleVelocity = (position - _lastMuzzlePosition) / Time.deltaTime;
                _muzzleVelocity = new Vector3(0, _muzzleVelocity.y, 0);
                _lastMuzzlePosition = position;
            }
        }

        private void ShowMuzzleFlash()
        {
            if(!_hasMuzzleFlash) return;
            
            var muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle.transform);
            Destroy(muzzleFlashInstance, 0.15f);
        }

        public void ShowWeapon(bool show)
        {
            root.SetActive(show);
            
            IsWeaponActive = show;
        }

        public bool TryShoot(Transform cameraTransform)
        {
            if (!(Time.time - _lastShootTime >= _shootData.shootDelay)) return false;
            
            _lastShootTime = Time.time;

            Vector3 shootDirection;

            for (int i = 0; i < _shootData.bulletCount; i++)
            {
                shootDirection = GetShootDirectionWithinSpread(muzzle);
                var newBullet = Instantiate(
                    bulletPrefab, muzzle.position, Quaternion.LookRotation(shootDirection));
                newBullet.Shoot(cameraTransform, Owner,_muzzleVelocity);
            }
            
            OnShoot?.Invoke();

            return true;
        }
        
        private Vector3 GetShootDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = _shootData.bulletSpreadAngle / 180f;
            Vector3 spreadWorldDir = Vector3.Slerp(shootTransform.forward, Random.insideUnitSphere,
                spreadAngleRatio);
            
            return spreadWorldDir;
        }

        public Vector3 GetRecoilMovement(bool isShooting)
        {
            return _recoilController.Calculate(isShooting);
        }

        public Vector3 GetAimOffset()
        {
            return WeaponData.AimOffset;
        }

        public float GetRecoilCameraForce()
        {
            return _recoilController.GetCameraForce();
        }
        
        private void OnShootHandler()
        {
            _soundManager.PlaySoundAtLocation(WeaponData.SoundClass,muzzle.position);
            ShowMuzzleFlash();
        }

    }

    [Serializable]
    public class WeaponCrosshairData
    {
        public Sprite sprite;
        [Range(1,200)]
        public float defaultSize;
        [Range(1,200)]
        public float aimSize;
        public Color defaultColor;
        public Color aimColor;
    }

    [Serializable]
    public class WeaponShootData
    {
        [Range(0,3)]
        public float shootDelay = 0.75f;
        [Range(1, 15)] 
        public int bulletCount = 1;
        [Range(0,180)]
        public float bulletSpreadAngle;
    }
}