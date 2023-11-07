using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Bullet
{
    public class WeaponsManager : MonoBehaviour
    {
        [SerializeField] private Camera weaponCamera;
        [SerializeField] private float bulletSpreadAngle;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private BulletStandard bulletPrefab;

        private Vector3 _muzzleVelocity;
        private Vector3 _lastMuzzlePosition;

        public Camera WeaponCamera => weaponCamera;

        private void Update()
        {
            if (Time.deltaTime > 0)
            {
                _muzzleVelocity = (shootPoint.position - _lastMuzzlePosition) / Time.deltaTime;
                _lastMuzzlePosition = shootPoint.position;
            }
        }

        public void Shoot()
        {
            var shootDirection = GetShootDirectionWithinSpread(shootPoint);
            BulletStandard newBullet =
                Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
            newBullet.Shoot(gameObject,_muzzleVelocity);
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