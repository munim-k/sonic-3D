using UnityEngine;

namespace RagdollEngine
{
    public class FisheyeCameraFollowState : PlayerCameraState
    {
        [Header("Camera Offset")]
        [SerializeField] Vector3 offset = new Vector3(0, 5, -10); // Relative to player position

        [Header("Fixed Rotation")]
        [SerializeField] Vector3 fixedEulerAngles = new Vector3(30, 0, 0); // Camera will always face this direction

        public override void Execute()
        {
            // Follow only the position of the player
            cameraTransform.position = modelTransform.position + offset;

            // Apply a fixed rotation
            cameraTransform.rotation = Quaternion.Euler(fixedEulerAngles);
        }

        public override void Enable()
        {
            // No initialization needed
        }

        public override void Transition()
        {
            transition = 0; // No transition behavior
        }
    }
}