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
            if (aimingState.Check())
            {
                aimingState.Execute();
                return;
            }
        
            float smoothSpeed = 5f;
        
            if (bossObject != null)
            {
                Vector3 playerPos = modelTransform.position;
                Vector3 bossPos = bossObject.transform.position;
        
                Vector3 toBoss = (bossPos - playerPos).normalized;
                float camDistance = offset.magnitude;
        
                Vector3 targetPos = playerPos - toBoss * camDistance + Vector3.up * offset.y;
                Quaternion targetRot = Quaternion.LookRotation(toBoss, Vector3.up);
        
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * smoothSpeed);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRot, Time.deltaTime * smoothSpeed);
            }
            else
            {
                Vector3 targetPos = modelTransform.position + offset;
                Quaternion targetRot = Quaternion.Euler(fixedEulerAngles);
        
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * smoothSpeed);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRot, Time.deltaTime * smoothSpeed);
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

        private void Update()
        {
            bossObject = GameObject.FindGameObjectWithTag("Boss");
        }
    }
}
