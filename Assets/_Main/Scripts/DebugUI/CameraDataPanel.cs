using System;
using System.Globalization;
using _Main.Scripts.Character.Components;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class CameraDataPanel : SliderPanelBase<CameraMovementData>
    {
        [SerializeField] private CameraVariable variable;

        protected override void SetNames()
        {
            VariableText.text = variable.ToString();
            gameObject.name = $"Camera{variable}";
        }
        
        public override void SetInit(CameraMovementData data)
        {
            SelfData = data;
            float sliderValue = 0;

            switch (variable)
            {
                case CameraVariable.StartFov:
                    sliderValue = data.startFov;
                    SetSliderLimits(40,110);
                    break;
                case CameraVariable.SprintFov:
                    sliderValue = data.sprintFov;
                    SetSliderLimits(60,130);
                    break;
                case CameraVariable.MoveAngle:
                    sliderValue = data.moveAngle;
                    SetSliderLimits(1,35);
                    break;
                case CameraVariable.TransitionTime:
                    sliderValue = data.transitionTime;
                    SetSliderLimits(1,50);
                    break;
                case CameraVariable.RestitutionTime:
                    sliderValue = data.restitutionTime;
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
                case CameraVariable.StartFov:
                    value = SelfData.startFov;
                    break;
                case CameraVariable.SprintFov:
                    value = SelfData.sprintFov;
                    break;
                case CameraVariable.MoveAngle:
                    value = SelfData.moveAngle;
                    break;
                case CameraVariable.TransitionTime:
                    value = SelfData.transitionTime;
                    break;
                case CameraVariable.RestitutionTime:
                    value = SelfData.restitutionTime;
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
                case CameraVariable.StartFov:
                    SelfData.startFov = value;
                    break;
                case CameraVariable.SprintFov:
                    SelfData.sprintFov = value;
                    break;
                case CameraVariable.MoveAngle:
                    SelfData.moveAngle = value;
                    break;
                case CameraVariable.TransitionTime:
                    SelfData.transitionTime = value;
                    break;
                case CameraVariable.RestitutionTime:
                    SelfData.restitutionTime = value;
                    break;
            }

            ValueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum CameraVariable
    {
        StartFov,
        SprintFov,
        MoveAngle,
        TransitionTime,
        RestitutionTime
    }
}