using System;
using System.Globalization;
using _Main.Scripts.Character.Components;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class BodyDataPanel : SliderPanelBase<BodyMovementData>
    {
        [SerializeField] private BodyVariable variable;
        
        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Body{variable}";
        }
        
        public override void SetInit(BodyMovementData data)
        {
            SelfData = data;
            float sliderValue = 0;
            
            switch (variable)
            {
                case BodyVariable.MaxGroundSpeed:
                    sliderValue = data.maxGroundSpeed;
                    SetSliderLimits(1,100);
                    break;
                case BodyVariable.MaxAirSpeed:
                    sliderValue = data.maxAirSpeed;
                    SetSliderLimits(1,100);
                    break;
                case BodyVariable.SprintModifier:
                    sliderValue = data.sprintSpeedModifier;
                    SetSliderLimits(1,10);
                    break;
                case BodyVariable.JumpForce:
                    sliderValue = data.jumpForce;
                    SetSliderLimits(1,50);
                    break;
                case BodyVariable.RotationSpeed:
                    sliderValue = data.rotationSpeed;
                    SetSliderLimits(1,100);
                    break;
            }
            
            Slider.value = (int)sliderValue;
            ValueText.text = sliderValue.ToString();
            VariableText.text = variable.ToString();
        }
        
        public override void ForceChange()
        {
            var value = 0f;
            
            switch (variable)
            {
                case BodyVariable.MaxGroundSpeed:
                    value = SelfData.maxGroundSpeed;
                    break;
                case BodyVariable.MaxAirSpeed:
                    value = SelfData.maxAirSpeed;
                    break;
                case BodyVariable.SprintModifier:
                    value = SelfData.sprintSpeedModifier;
                    break;
                case BodyVariable.JumpForce:
                    value = SelfData.jumpForce;
                    break;
                case BodyVariable.RotationSpeed:
                    value = SelfData.rotationSpeed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Slider.value = (int)value;
            Slider.onValueChanged.Invoke(value);
        }

        protected override void OnValueChangeHandler(float value)
        {
            if (SelfData == null) return;
            
            switch (variable)
            {
                case BodyVariable.MaxGroundSpeed:
                    SelfData.maxGroundSpeed = value;
                    break;
                case BodyVariable.MaxAirSpeed:
                    SelfData.maxAirSpeed = value;
                    break;
                case BodyVariable.SprintModifier:
                    SelfData.sprintSpeedModifier = value;
                    break;
                case BodyVariable.JumpForce:
                    SelfData.jumpForce = value;
                    break;
                case BodyVariable.RotationSpeed:
                    SelfData.rotationSpeed = value;
                    break;
            }
            
            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum BodyVariable
    {
        MaxGroundSpeed,
        MaxAirSpeed,
        SprintModifier,
        JumpForce,
        RotationSpeed
    }
}