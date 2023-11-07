using _Main.Scripts.Jetpack;
using UnityEngine;

namespace _Main.Scripts.Character.Components
{
    [CreateAssetMenu(fileName = "PlayerComponentsData", menuName = "Scriptable Objects/Player/Components Data", order = 0)]
    public class PlayerComponentsData : ScriptableObject
    {
        public BodyMovementData bodyData;
        public CameraMovementData cameraData;
        [Tooltip("Weapon movement when player is moving")]
        public BobData bobData;
        [Tooltip("Weapon movement when the player rotates the camera")]
        public SwayData swayData;
        public RecoilData recoilData;
        public JetpackData jetpackData;


        public void ResetDefault()
        {
            //Body
            bodyData.maxGroundSpeed = 10f;
            bodyData.maxAirSpeed = 8f;
            bodyData.sprintSpeedModifier = 2f;
            bodyData.jumpForce = 10f;
            bodyData.rotationSpeed = 20f;
            //Camera
            cameraData.startFov = 60f;
            cameraData.sprintFov = 70f;
            cameraData.moveAngle = 15f;
            cameraData.transitionTime = 15f;
            cameraData.restitutionTime = 25f;
            //Bod
            bobData.bobSharpness = 10f;
            bobData.bobFrequency = 10f;
            bobData.defaultBobAmount = 5f;
            bobData.alternativeBobAmount = 2f;
            //Sway
            swayData.amount = 10;
            swayData.smoothDamp = 5;
            swayData.resetSmoothing = 5;
            swayData.clampX = 5;
            swayData.clampY = 5;
            //Recoil
            recoilData.force = 1;
            recoilData.maxDistance = 5;
            recoilData.sharpness = 50;
            recoilData.restitutionSharpness = 10;
            //Jetpack
            jetpackData.acceleration = 25;
            jetpackData.consumeDuration = 1.5f;
            jetpackData.refillDuration = 2f;
            jetpackData.downwardVelocityCancelingFactor = 1f;
            jetpackData.refillDelay = 1f;
        }

    }
}