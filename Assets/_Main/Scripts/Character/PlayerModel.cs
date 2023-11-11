using System;
using _Main.Scripts.Weapons;
using _Main.Scripts.Character.Components;
using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(WeaponsManager))]
    public class PlayerModel : MonoBehaviour
    {
        private MovementController _movementController;
        private WeaponsManager _weaponsManager;

        private Vector3 _startPos;
        private float _movementMultiplier = 1;
        
        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            _weaponsManager = GetComponent<WeaponsManager>();
        }

        private void Start()
        {
            _startPos = transform.position;
        }
        
        private void Update()
        {
            if (transform.position.y < -100f)
            {
                transform.position = _startPos;
            }
        }

        public void Move(Vector2 input)
        {
            _movementController.Move(input * _movementMultiplier);
        }
        
        public void Look(Vector2 mouseInput)
        {
            _movementController.Look(mouseInput);
        }

        public void Jump()
        {
            _movementController.Jump();
        }

        public void Sprint(bool input)
        {
            _movementController.SetSprint(input);
        }

        public void Shoot(bool isShooting)
        {
            _weaponsManager.HandleShoot(isShooting);
        }
        
        public void Aim(bool isAiming)
        {
            var canAim = _weaponsManager.TryAim(isAiming);
            _movementController.SetFov(FovType.Aim, canAim);
            _movementMultiplier = canAim ? 0.5f : 1;

        }

        public void Switch()
        {
            _weaponsManager.HandleSwitch();
        }

        public void UseJetpack(bool hasPressed, bool isPressed)
        {
            _movementController.UseJetpack(hasPressed,isPressed);
        }


    }
}