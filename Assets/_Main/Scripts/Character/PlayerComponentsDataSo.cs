using _Main.Scripts.Jetpack;
using _Main.Scripts.Weapons;
using _Main.Scripts.Weapons.Components;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    [CreateAssetMenu(fileName = "PlayerComponentsData", menuName = "Scriptable Objects/Player/Components Data", order = 0)]
    public class PlayerComponentsDataSo : ScriptableObject
    {
        [SerializeField]
        private BodyMovementData bodyData;
        [SerializeField]
        private CameraMovementData cameraData;
        [SerializeField]
        private JetpackData jetpackData;

        [Space] 
        
        [SerializeField] [Range(1, 50)]
        private float cameraForceAtGroundImpact = 15;

        public BodyMovementData BodyData => bodyData;

        public CameraMovementData CameraData => cameraData;

        public JetpackData JetpackData => jetpackData;

        public float CameraForceAtGroundImpact => cameraForceAtGroundImpact;
    }
}