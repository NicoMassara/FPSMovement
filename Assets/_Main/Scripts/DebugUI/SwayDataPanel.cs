using System;
using System.Globalization;
using _Main.Scripts.Character.Components;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class SwayDataPanel : SliderPanelBase<SwayData>
    {
        [SerializeField] private SwayVariable variable;
        
        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Sway{variable}";
        }
        
        public override void SetInit(SwayData data)
        {
            SelfData = data;
            float sliderValue = 0;
            
            switch (variable)
            {
                case SwayVariable.Amount:
                    sliderValue = SelfData.amount;
                    SetSliderLimits(1,20);
                    break;
                case SwayVariable.SmoothDamp:
                    sliderValue = SelfData.smoothDamp;
                    SetSliderLimits(1,10);
                    break;
                case SwayVariable.ResetSmoothing:
                    sliderValue = SelfData.resetSmoothing;
                    SetSliderLimits(1,10);
                    break;
                case SwayVariable.ClampX:
                    sliderValue = SelfData.clampX;
                    SetSliderLimits(1,10);
                    break;
                case SwayVariable.ClampY:
                    sliderValue = SelfData.clampY;
                    SetSliderLimits(1,10);
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
                case SwayVariable.Amount:
                    value = SelfData.amount;
                    break;
                case SwayVariable.SmoothDamp:
                    value = SelfData.smoothDamp;
                    break;
                case SwayVariable.ResetSmoothing:
                    value = SelfData.resetSmoothing;
                    break;
                case SwayVariable.ClampX:
                    value = SelfData.clampX;
                    break;
                case SwayVariable.ClampY:
                    value = SelfData.clampY;
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
                case SwayVariable.Amount:
                    SelfData.amount = value;
                    break;
                case SwayVariable.SmoothDamp:
                    SelfData.smoothDamp = value;
                    break;
                case SwayVariable.ResetSmoothing:
                    SelfData.resetSmoothing = value;
                    break;
                case SwayVariable.ClampX:
                    SelfData.clampX = value;
                    break;
                case SwayVariable.ClampY:
                    SelfData.clampY = value;
                    break;
            }
            
            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum SwayVariable
    {
        Amount,
        SmoothDamp,
        ResetSmoothing,
        ClampX,
        ClampY
    }
}