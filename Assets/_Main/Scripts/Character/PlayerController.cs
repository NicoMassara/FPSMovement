
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
        [SerializeField] private GameObject debugPanel;
        [Range(1,50)][SerializeField] private float controlSensitivityMultiplier = 15;
        
        private PlayerModel _model;
        private PlayerControls.MovementActions _movementActions;
        private PlayerControls.WeaponsActions _weaponsActions;
        private bool _blockInput;

        private bool _isGamepad;
        
        private void Awake()
        {
            _model = GetComponent<PlayerModel>();
            debugPanel.SetActive(false);
            
            var playerControls = new PlayerControls();
            playerControls.Enable();
            _movementActions = playerControls.Movement;
            _weaponsActions = playerControls.Weapons;
        }

        private void Start()
        {
            _movementActions.Jump.performed += _ => _model.Jump();
        }

        private void Update()
        {
            var mouseInput = _movementActions.AimAxis.ReadValue<Vector2>();
            var moveInput = _movementActions.MovementAxis.ReadValue<Vector2>();

            mouseInput = _isGamepad ? mouseInput * controlSensitivityMultiplier : mouseInput;
            
            _model.Move(moveInput);
            _model.Look(_blockInput ? Vector2.zero : mouseInput);
            _model.Sprint(_movementActions.Sprint.IsPressed());
            _model.Shoot(_weaponsActions.Shoot.IsPressed() && !_blockInput);
            _model.UseJetpack(_movementActions.Jump.WasPressedThisFrame(),_movementActions.Jump.IsPressed());
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                _blockInput = !_blockInput;
                
                debugPanel.SetActive(_blockInput);
            }
        }

        public void OnControlsChanged(PlayerInput input)
        {
            _isGamepad = input.currentControlScheme.Equals("Gamepad");
        }
    }
}