using UnityEngine;

namespace RagdollEngine
{
    public class FisheyeCameraFollowState : PlayerCameraState
    {
        [Header("Camera Offset")]
        [SerializeField] Vector3 offset = new Vector3(0, 5, -10); // Relative to player position

        [Header("Fixed Rotation (used if no boss)")]
        [SerializeField] Vector3 fixedEulerAngles = new Vector3(30, 0, 0); // Camera will always face this direction

        [Header("References")]
        [SerializeField] AimingPlayerCameraState aimingState; // Reference to the aiming state

        private GameObject bossObject; // Reference to the boss object

        private void Start()
        {
            bossObject = GameObject.FindGameObjectWithTag("Boss");
        }

        public override void Execute()
        {
            // If player is aiming, delegate to aiming camera
            if (aimingState.Check())
            {
                aimingState.Execute();
                return;
            }

            if (bossObject != null)
            {
                Vector3 playerPos = modelTransform.position;
                Vector3 bossPos = bossObject.transform.position;

                // Direction from player to boss
                Vector3 toBoss = (bossPos - playerPos).normalized;

                // Offset distance (magnitude of original offset)
                float camDistance = offset.magnitude;

                // Place camera behind the player, opposite to the boss direction
                Vector3 cameraPos = playerPos - toBoss * camDistance + Vector3.up * offset.y;

                // Rotate camera to look at the boss
                Quaternion lookRotation = Quaternion.LookRotation(toBoss, Vector3.up);

                cameraTransform.position = cameraPos;
                cameraTransform.rotation = lookRotation;
            }
            else
            {
                // Default behavior: follow player with fixed rotation
                cameraTransform.position = modelTransform.position + offset;
                cameraTransform.rotation = Quaternion.Euler(fixedEulerAngles);
            }
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

            transition = 0;
        }
    }
}
