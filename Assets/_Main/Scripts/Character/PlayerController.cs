
using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(PlayerModel))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject debugPanel;
        
        private PlayerModel _model;
        private bool _blockInput;
        
        private void Awake()
        {
            _model = GetComponent<PlayerModel>();
            debugPanel.SetActive(false);
        }

        private void Update()
        {
            var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            var moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveInput = Vector2.ClampMagnitude(moveInput, 1);
            
            if(Input.GetKeyDown(KeyCode.Space)) _model.Jump();
            
            _model.Move(moveInput);
            _model.Rotate(_blockInput ? Vector2.zero : mouseInput);
            _model.Sprint(Input.GetKey(KeyCode.LeftShift));
            _model.Shoot(Input.GetKeyDown(KeyCode.Mouse0) && !_blockInput);
            _model.UseJetpack(Input.GetKeyDown(KeyCode.Space),Input.GetKey(KeyCode.Space));
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                _blockInput = !_blockInput;
                
                debugPanel.SetActive(_blockInput);
            }
        }
    }
}