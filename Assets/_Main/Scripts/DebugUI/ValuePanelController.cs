using System;
using _Main.Scripts.Character;
using _Main.Scripts.Character.Components;
using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.DebugUI
{
    public class ValuePanelController : MonoBehaviour
    {
        [SerializeField] private CameraDataPanel[] cameraData;
        [SerializeField] private BodyDataPanel[] bodyData;
        [SerializeField] private RecoilDataPanel[] recoilData;
        [SerializeField] private BobDataPanel[] bobData;
        [SerializeField] private SwayDataPanel[] swayData;
        [SerializeField] private JetpackDataPanel[] jetpackData;
        [Space]
        [SerializeField] private PlayerComponentsData data;

        private void Start()
        {
            foreach (var t in cameraData)
            {
                t.SetInit(data.cameraData);
            }
            
            foreach (var t in bodyData)
            {
                t.SetInit(data.bodyData);
            }
            
            foreach (var t in recoilData)
            {
                t.SetInit(data.recoilData);
            }
            
            foreach (var t in bobData)
            {
                t.SetInit(data.bobData);
            }
            
            foreach (var t in swayData)
            {
                t.SetInit(data.swayData);
            }
            
            foreach (var t in jetpackData)
            {
                t.SetInit(data.jetpackData);
            }
        }

        public void ResetDefault()
        {
            data.ResetDefault();
            
            foreach (var t in cameraData)
            {
                t.ForceChange();
            }
            
            foreach (var t in bodyData)
            {
                t.ForceChange();
            }
            
            foreach (var t in recoilData)
            {
                t.ForceChange();
            }
            
            foreach (var t in bobData)
            {
                t.ForceChange();
            }
            
            foreach (var t in swayData)
            {
                t.ForceChange();
            }
            
            foreach (var t in jetpackData)
            {
                t.ForceChange();
            }
        }

        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
}