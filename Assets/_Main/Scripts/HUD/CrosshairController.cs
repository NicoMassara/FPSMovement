using System;
using _Main.Scripts.Character;
using _Main.Scripts.Character.Components;
using _Main.Scripts.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.HUD
{
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private Sprite nullCrosshairSprite;
        [SerializeField] private float crosshairUpdateSharpness = 5f;
        [SerializeField] private WeaponsManager weaponsManager;

        private bool _isCrossHairEnable;
        private bool _isPointingAtEnemy;
        private bool _wasPointingAtEnemy;
        private WeaponCrosshairData _crosshairData;
        private float _currentSize;
        private Color _currentColor;
        private RectTransform _crossHairRectTransform;

        private void Awake()
        {
            _crossHairRectTransform = crosshairImage.GetComponent<RectTransform>();
            
            weaponsManager.OnWeaponSwitched += OnWeaponSwitchedHandler;
            weaponsManager.OnChangeAim += OnChangeAimHandler;
        }

        private void Start()
        {
            OnWeaponSwitchedHandler(weaponsManager.GetCurrentWeapon());
        }

        private void Update()
        {
            _isPointingAtEnemy = weaponsManager.IsPointingAtEnemy;

            UpdateCrosshair();

            _wasPointingAtEnemy = _isPointingAtEnemy;
        }
        
        private void UpdateCrosshair(bool force = false)
        {
            if(_crosshairData.sprite == null) return;

            if (!_isCrossHairEnable)
            {
                crosshairImage.color = Color.Lerp(crosshairImage.color, Color.clear, 
                    Time.deltaTime * crosshairUpdateSharpness);
                
                return;
            }
            if ((force || !_wasPointingAtEnemy) && _isPointingAtEnemy)
            {
                _currentSize = _crosshairData.aimSize;
                _currentColor = _crosshairData.aimColor;
            }
            else if ((force || _wasPointingAtEnemy) 
                     && (!_isPointingAtEnemy))
            {
                _currentSize = _crosshairData.defaultSize;
                _currentColor = _crosshairData.defaultColor;
            }
            
            crosshairImage.color = Color.Lerp(crosshairImage.color, _currentColor,
                Time.deltaTime * crosshairUpdateSharpness);
            
            _crossHairRectTransform.sizeDelta = Mathf.Lerp(_crossHairRectTransform.sizeDelta.x, _currentSize,
                Time.deltaTime * crosshairUpdateSharpness) * Vector2.one;
        }


        private void OnWeaponSwitchedHandler(WeaponController weapon)
        {
            if (weapon)
            {
                crosshairImage.enabled = true;
                _crosshairData = weapon.WeaponData.CrosshairData;
                _currentSize = _crosshairData.defaultSize;
                _currentColor = _crosshairData.defaultColor;
                crosshairImage.sprite = _crosshairData.sprite;
            }
            else
            {
                if (nullCrosshairSprite)
                {
                    crosshairImage.sprite = nullCrosshairSprite;
                }
                else
                {
                    crosshairImage.enabled = false;
                }
            }
            
            UpdateCrosshair(true);
        }
        
        private void OnChangeAimHandler(bool isAiming)
        {
            _isCrossHairEnable = !isAiming;
        }
    }
}