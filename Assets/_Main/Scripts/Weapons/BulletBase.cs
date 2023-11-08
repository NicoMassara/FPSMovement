using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Weapons
{
    public class BulletBase : MonoBehaviour
    {
        public GameObject Owner { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }

        protected Transform SelfTransform { get; set; }
        protected Transform CameraTransform { get; private set; }

        public UnityAction OnShoot;

        public void Shoot(Transform weaponCamera, GameObject owner, Vector3 muzzleVelocity)
        {
            Owner = owner;
            CameraTransform = weaponCamera;
            SelfTransform = transform;
            InitialPosition = SelfTransform.position;
            InitialDirection = SelfTransform.forward;
            InheritedMuzzleVelocity = muzzleVelocity;
            
            OnShoot?.Invoke();
        }
    }
}