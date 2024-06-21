
using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(PlayerModel))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] [Range(1,10)]
        private float mouseSensitivity = 5;
        [SerializeField] [Range(1,10)]
        private int gamepadSensitivity = 5;
        
        private PlayerModel _model;
        private PlayerControls.MovementActions _movementActions;
        private PlayerControls.WeaponsActions _weaponsActions;

        private float _currentSensitivity;

        private bool _isGamepad;
        
        private void Awake()
        {
            _model = GetComponent<PlayerModel>();
            
            var playerControls = new PlayerControls();
            playerControls.Enable();
            _movementActions = playerControls.Movement;
            _weaponsActions = playerControls.Weapons;
        }

        private void Start()
        {
            _movementActions.Jump.performed += _ => _model.Jump();
            _weaponsActions.Cycle.performed += _ => _model.Switch();
        }

        private void Update()
        {
            var mouseInput = _movementActions.AimAxis.ReadValue<Vector2>();
            var moveInput = _movementActions.MovementAxis.ReadValue<Vector2>();
            
            _model.Move(moveInput);
            _model.Look(mouseInput*_currentSensitivity);
            _model.Sprint(_movementActions.Sprint.IsPressed());
            _model.Shoot(_weaponsActions.Shoot.IsPressed());
            _model.Aim(_weaponsActions.Aim.IsPressed());
            _model.UseJetpack(_movementActions.Jump.WasPressedThisFrame(),_movementActions.Jump.IsPressed());
        }

        public void OnControlsChanged(PlayerInput input)
        {
            _isGamepad = input.currentControlScheme.Equals("Gamepad");
            
            //It's divided by 50 and 2 cause it makes easier to control and set sensitivity
            _currentSensitivity = _isGamepad ? 
                (float)gamepadSensitivity / 2 : mouseSensitivity / 50;
        }
    }
}