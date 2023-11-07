using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.DebugUI
{
    [ExecuteInEditMode]
    public abstract class SliderPanelBase<T> : MonoBehaviour
    {
        protected TMP_Text VariableText { get; private set; }
        protected TMP_Text ValueText { get; private set; }
        protected Slider Slider { get; private set; }

        protected T SelfData;

        public abstract void SetInit(T data);

        private void Awake()
        {
            VariableText = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>(); 
            ValueText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>(); 
            Slider = transform.GetChild(1).GetChild(0).GetComponent<Slider>();

            Slider.onValueChanged.AddListener(OnValueChangeHandler);
            SetNames();
        }

        public abstract void ForceChange();


        protected abstract void SetNames();

        protected void SetSliderLimits(int min, int max)
        {
            Slider.minValue = min;
            Slider.maxValue = max;
        }


        protected abstract void OnValueChangeHandler(float value);
    }
}