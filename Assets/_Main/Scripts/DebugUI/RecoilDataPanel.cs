using System;
using System.Globalization;
using _Main.Scripts.Character.Components;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class RecoilDataPanel : SliderPanelBase<RecoilData>
    {
        [SerializeField] private RecoilVariable variable;
        
        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Recoil{variable}";
        }
        
        public override void SetInit(RecoilData data)
        {
            SelfData = data;
            float sliderValue = 0;
            
            switch (variable)
            {
                case RecoilVariable.Force:
                    sliderValue = SelfData.force;
                    SetSliderLimits(0,10);
                    break;
                case RecoilVariable.MaxDistance:
                    sliderValue = SelfData.maxDistance;
                    SetSliderLimits(1,10);
                    break;
                case RecoilVariable.Sharpness:
                    sliderValue = SelfData.sharpness;
                    SetSliderLimits(10,100);
                    break;
                case RecoilVariable.RestitutionSharpness:
                    sliderValue = SelfData.restitutionSharpness;
                    SetSliderLimits(1,50);
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
                case RecoilVariable.Force:
                    value = SelfData.force;
                    break;
                case RecoilVariable.MaxDistance:
                    value = SelfData.maxDistance;
                    break;
                case RecoilVariable.Sharpness:
                    value = SelfData.sharpness;
                    break;
                case RecoilVariable.RestitutionSharpness:
                    value = SelfData.restitutionSharpness;
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
                case RecoilVariable.Force:
                    SelfData.force = value;
                    break;
                case RecoilVariable.MaxDistance:
                    SelfData.maxDistance = value;
                    break;
                case RecoilVariable.Sharpness:
                    SelfData.sharpness = value;
                    break;
                case RecoilVariable.RestitutionSharpness:
                    SelfData.restitutionSharpness = value;
                    break;
            }
            
            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum RecoilVariable
    {
        Force,
        MaxDistance,
        Sharpness,
        RestitutionSharpness
    }
}