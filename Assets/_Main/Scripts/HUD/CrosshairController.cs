using System;
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
        [SerializeField] private MovementController movementController;

        private bool _isPointingAtEnemy, _isMoving;
        private bool _wasPointingAtEnemy;
        private CrossHairData _currentData, _defaultData, _aimData;
        private RectTransform _crossHairRectTransform;

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
            _isPointingAtEnemy = weaponsManager.IsPointingAtEnemy;
            _isMoving = movementController.GetIsMoving();

            UpdateCrosshair();

            _wasPointingAtEnemy = _isPointingAtEnemy;
        }
        
        private void UpdateCrosshair(bool force = false)
        {
            if(_defaultData.sprite == null) return;

            if ((force || !_wasPointingAtEnemy) && _isPointingAtEnemy)
            {
                _currentData = _aimData;
                crosshairImage.sprite = _currentData.sprite;
            }
            else if ((force || _wasPointingAtEnemy) 
                     && (!_isPointingAtEnemy))
            {
                _currentData = _defaultData;
                crosshairImage.sprite = _currentData.sprite;
            }
            
            crosshairImage.color = Color.Lerp(crosshairImage.color, _currentData.color,
                Time.deltaTime * crosshairUpdateSharpness);
            
            _crossHairRectTransform.sizeDelta = Mathf.Lerp(_crossHairRectTransform.sizeDelta.x, _currentData.size,
                Time.deltaTime * crosshairUpdateSharpness) * Vector2.one;
        }
        

        private void OnWeaponSwitchedHandler(WeaponController weapon)
        {
            if (weapon)
            {
                crosshairImage.enabled = true;
                _currentData = weapon.defaultCrosshair;
                _defaultData = weapon.defaultCrosshair;
                _aimData = weapon.aimCrosshair;
                _crossHairRectTransform = crosshairImage.GetComponent<RectTransform>();
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
    }
}