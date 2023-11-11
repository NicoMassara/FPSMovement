using System;
using _Main.Scripts.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.HUD
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private float crosshairUpdateSharpness = 5f;
        [SerializeField] private WeaponsManager weaponsManager;

        private bool _wasPointingAtEnemy;
        private RectTransform _crossHairRectTransform;
        private CrossHairData _currentData;
        private CrossHairData _defaultData;
        private CrossHairData _aimData;

        private void Awake()
        {
            weaponsManager.OnWeaponSwitched += OnWeaponSwitchedHandler;
        }

        private void Start()
        {
            OnWeaponSwitchedHandler(weaponsManager.GetCurrentWeapon());
        }

        private void Update()
        {
            UpdateCrossHairPointingAtEnemy();
            _wasPointingAtEnemy = weaponsManager.IsPointingAtEnemy;
        }
        
        private void UpdateCrossHairPointingAtEnemy(bool force = false)
        {
            var isPointingAtEnemy = weaponsManager.IsPointingAtEnemy;

            if ((force || !_wasPointingAtEnemy) && isPointingAtEnemy)
            {
                _currentData = _aimData;
                _crossHairRectTransform.sizeDelta = _currentData.Size * Vector2.one;
            }
            else if ((force || _wasPointingAtEnemy) && !isPointingAtEnemy)
            {
                _currentData = _defaultData;
                _crossHairRectTransform.sizeDelta = _currentData.Size * Vector2.one;
            }
            
            crosshairImage.color = Color.Lerp(crosshairImage.color, _currentData.Color,
                Time.deltaTime * crosshairUpdateSharpness);
            
            _crossHairRectTransform.sizeDelta = Mathf.Lerp(_crossHairRectTransform.sizeDelta.x, _currentData.Size,
                Time.deltaTime * crosshairUpdateSharpness) * Vector2.one;
        }
        
        private void OnWeaponSwitchedHandler(WeaponController weapon)
        {
            if(!weapon) return;
            
            _currentData = weapon.defaultCrosshair;
            _defaultData = weapon.defaultCrosshair;
            _aimData = weapon.aimCrosshair;
            _crossHairRectTransform = crosshairImage.GetComponent<RectTransform>();

            UpdateCrossHairPointingAtEnemy(true);
        }
    }
}