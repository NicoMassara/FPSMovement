using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private int bulletCount;
        [SerializeField] private float bulletSpreadAngle;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private BulletStandard bulletPrefab;
        [SerializeField] private float shootDelay = 0.75f;
        
        private float _lastShootTime;
        private bool _isShooting;
        
        private Vector3 _muzzleVelocity;
        private Vector3 _lastMuzzlePosition;
        
        public GameObject Owner { get; set; }

        public UnityAction OnShoot;

        private void Update()
        {
            if (Time.deltaTime > 0)
            {
                var position = shootPoint.position;
                _muzzleVelocity = (position - _lastMuzzlePosition) / Time.deltaTime;
                _lastMuzzlePosition = position;
            }
        }

        public bool TryShoot(Transform cameraTransform)
        {
            if (!(Time.time - _lastShootTime >= shootDelay)) return false;
            
            _lastShootTime = Time.time;

            var shootDirection = Vector3.zero;

            for (int i = 0; i < bulletCount; i++)
            {
                shootDirection = GetShootDirectionWithinSpread(shootPoint);
                var newBullet = Instantiate(
                    bulletPrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
                newBullet.Shoot(cameraTransform, Owner,_muzzleVelocity);
            }
            
            OnShoot?.Invoke();

            return true;
        }
        
        public Vector3 GetShootDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = bulletSpreadAngle / 180f;
            Vector3 spreadWorldDir = Vector3.Slerp(shootTransform.forward, Random.insideUnitSphere,
                spreadAngleRatio);
            
            return spreadWorldDir;
        }
    }
}