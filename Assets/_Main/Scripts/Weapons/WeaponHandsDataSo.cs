using _Main.Scripts.Weapons.Components;
using UnityEngine;

namespace _Main.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Hands Data", menuName = "Scriptable Objects/Weapons/Hands Data", order = 0)]
    public class WeaponHandsDataSo : ScriptableObject
    {
        [SerializeField]
        private WeaponBobData bobData;
        [SerializeField]
        private WeaponSwayData swayData;
        [Space]
        [SerializeField] [Range(0, 2)] 
        private float switchDelay = 0.5f;
        [SerializeField] [Range(0, 30)] 
        private float aimSpeed = 10f;

        public WeaponBobData BobData => bobData;

        public WeaponSwayData SwayData => swayData;

        public float SwitchDelay => switchDelay;

        public float AimSpeed => aimSpeed;
    }
}