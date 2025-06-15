using UnityEngine;

namespace RagdollEngine
{
    public class FisheyeCameraFollowState : PlayerCameraState
    {
        [Header("Camera Offset")]
        [SerializeField] Vector3 offset = new Vector3(0, 5, -10); // Relative to player position

        [Header("Fixed Rotation")]
        [SerializeField] Vector3 fixedEulerAngles = new Vector3(30, 0, 0); // Camera will always face this direction

        [Header("References")]
        [SerializeField] AimingPlayerCameraState aimingState; // Reference to the aiming state

        public override void Execute()
        {
            if (aimingState.Check()) // If aiming input is active
            {
                aimingState.Execute(); // Delegate control to the aiming state
                return;
            }

            // Default fisheye follow behavior
            cameraTransform.position = modelTransform.position + offset;
            cameraTransform.rotation = Quaternion.Euler(fixedEulerAngles);
        }

        public override void Enable()
        {
            if (aimingState.Check())
            {
                aimingState.Enable();
                return;
            }

            // No initialization needed for fisheye
        }

        public override void Transition()
        {
            if (aimingState.Check())
            {
                aimingState.Transition();
                return;
            }

            transition = 0; // No transition behavior for fisheye
        }
    }
}
