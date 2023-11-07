using System;
using System.Globalization;
using _Main.Scripts.Character.Components;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class BobDataPanel : SliderPanelBase<BobData>
    {
        [SerializeField] private BobVariable variable;
        
        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Bob{variable}";
        }
        
        public override void SetInit(BobData data)
        {
            SelfData = data;
            float sliderValue = 0;


            switch (variable)
            {
                case BobVariable.Sharpness:
                    sliderValue = data.bobSharpness;
                    SetSliderLimits(1,20);
                    break;
                case BobVariable.Frequency:
                    sliderValue = data.bobFrequency;
                    SetSliderLimits(1,20);
                    break;
                case BobVariable.DefaultAmount:
                    sliderValue = data.defaultBobAmount;
                    SetSliderLimits(1,10);
                    break;
                case BobVariable.AlternativeAmount:
                    sliderValue = data.alternativeBobAmount;
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
                case BobVariable.Sharpness:
                    value = SelfData.bobSharpness;
                    break;
                case BobVariable.Frequency:
                    value = SelfData.bobFrequency;
                    break;
                case BobVariable.DefaultAmount:
                    value = SelfData.defaultBobAmount;
                    break;
                case BobVariable.AlternativeAmount:
                    value = SelfData.alternativeBobAmount;
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
                case BobVariable.Sharpness:
                    SelfData.bobSharpness = value;
                    break;
                case BobVariable.Frequency:
                    SelfData.bobFrequency = value;
                    break;
                case BobVariable.DefaultAmount:
                    SelfData.defaultBobAmount = value;
                    break;
                case BobVariable.AlternativeAmount:
                    SelfData.alternativeBobAmount = value;
                    break;
            }
            
            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum BobVariable
    {
        Sharpness,
        Frequency,
        DefaultAmount,
        AlternativeAmount
    }
}