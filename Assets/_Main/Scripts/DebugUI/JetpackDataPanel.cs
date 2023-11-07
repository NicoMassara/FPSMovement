using System;
using System.Globalization;
using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class JetpackDataPanel : SliderPanelBase<JetpackData>
    {
        [SerializeField] private JetpackVariable variable;

        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Jetpack{variable}";
        }
        
        public override void SetInit(JetpackData data)
        {
            SelfData = data;
            float sliderValue = 0;

            switch (variable)
            {
                case JetpackVariable.Acceleration:
                    sliderValue = data.acceleration;
                    SetSliderLimits(10,50);
                    break;
                case JetpackVariable.ConsumeDuration:
                    sliderValue = data.consumeDuration;
                    SetSliderLimits(1,10);
                    break;
                case JetpackVariable.RefillDuration:
                    sliderValue = data.refillDuration;
                    SetSliderLimits(0,15);
                    break;
                case JetpackVariable.VelCancelingFactor:
                    sliderValue = data.downwardVelocityCancelingFactor;
                    SetSliderLimits(0,1);
                    break;
                case JetpackVariable.RefillDelay:
                    sliderValue = data.refillDelay;
                    SetSliderLimits(0,10);
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
                case JetpackVariable.Acceleration:
                    value = SelfData.acceleration;
                    break;
                case JetpackVariable.ConsumeDuration:
                    value = SelfData.consumeDuration;
                    break;
                case JetpackVariable.RefillDuration:
                    value = SelfData.refillDuration;
                    break;
                case JetpackVariable.VelCancelingFactor:
                    value = SelfData.downwardVelocityCancelingFactor;
                    break;
                case JetpackVariable.RefillDelay:
                    value = SelfData.refillDelay;
                    break;
            }
            
            Slider.value = (int)value;
            Slider.onValueChanged.Invoke(value);
        }

        protected override void OnValueChangeHandler(float value)
        {
            if (SelfData == null) return;

            switch (variable)
            {
                case JetpackVariable.Acceleration:
                    SelfData.acceleration = value;
                    break;
                case JetpackVariable.ConsumeDuration:
                    SelfData.consumeDuration = value;
                    break;
                case JetpackVariable.RefillDuration:
                    SelfData.refillDuration = value;
                    break;
                case JetpackVariable.VelCancelingFactor:
                    SelfData.downwardVelocityCancelingFactor = value;
                    break;
                case JetpackVariable.RefillDelay:
                    SelfData.refillDelay = value;
                    break;
            }

            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum JetpackVariable
    {
        Acceleration,
        ConsumeDuration,
        RefillDuration,
        VelCancelingFactor,
        RefillDelay
    }
}