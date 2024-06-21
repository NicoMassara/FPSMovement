using _Main.Scripts.Sounds;
using _Main.Scripts.Weapons.Components;
using UnityEngine;

namespace _Main.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Objects/Weapons/Data", order = 0)]
    public class WeaponDataSo : ScriptableObject
    {
        [SerializeField] private string weaponName = "Weapon";
        [Header("Shoot Values")]
        [SerializeField]
        private WeaponShootData shootData;
        [SerializeField]
        private WeaponRecoilData recoilData;
        [SerializeField]
        private WeaponCrosshairData crosshairData;
        [SerializeField] 
        private Vector3 aimOffset;

        [Space] 
        [SerializeField] 
        private SoundClassSo soundClass;
        

        public string WeaponName => weaponName;
        public WeaponShootData ShootData => shootData;
        public WeaponRecoilData RecoilData => recoilData;
        public WeaponCrosshairData CrosshairData => crosshairData;
        public Vector3 AimOffset => aimOffset;

        public SoundClassSo SoundClass => soundClass;
    }
}