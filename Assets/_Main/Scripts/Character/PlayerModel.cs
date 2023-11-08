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
            _movementController.Move(input);
        }
        
        public void Rotate(Vector2 mouseInput)
        {
            _movementController.Rotate(mouseInput);
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

        public void UseJetpack(bool hasPressed, bool isPressed)
        {
            _movementController.UseJetpack(hasPressed,isPressed);
        }

    }
}