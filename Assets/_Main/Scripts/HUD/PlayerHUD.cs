using System;
using _Main.Scripts.Character;
using _Main.Scripts.Character.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Image jetpackFuelBar;
        [SerializeField] private TMP_Text velocityText;

        [SerializeField] private MovementController playerMovement;
        private void Update()
        {
            jetpackFuelBar.fillAmount = playerMovement.GetJetpackFuel();
            velocityText.text = $"Velocity: {playerMovement.GetVelocity()}";
        }
    }
}