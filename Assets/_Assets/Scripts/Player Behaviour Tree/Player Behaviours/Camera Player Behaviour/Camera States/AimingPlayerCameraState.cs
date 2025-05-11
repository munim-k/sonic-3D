using UnityEngine;

namespace RagdollEngine
{
    public class AimingPlayerCameraState : PlayerCameraState
    {
        // Zoom-related variables
        [Header("Zoom")]
        [SerializeField] float defaultDistance; // Default distance of the camera from the player.
        [SerializeField] Vector3 positionOffset; // Offset for the camera's position relative to the player.
        [SerializeField] float minDistance; // Minimum zoom distance.
        [SerializeField] float maxDistance; // Maximum zoom distance.
        [SerializeField] float zoomSensitivity; // Sensitivity of zoom input.
        [SerializeField] float zoomFalloff; // Falloff factor for zooming (affects how zoom slows down near limits).
        [SerializeField, Range(0, 1)] float zoomSmoothness; // Smoothness of zoom transitions.

        float distance; // Current target distance for the camera.
        float currentDistance; // Smoothed distance for the camera.

     
        // Normal-related variables
        [Header("Normal")]
        [SerializeField, Range(0, 1)] float normalSmoothness; // Smoothness of the camera's alignment with the ground normal.
        Vector3 normal; // Current ground normal the camera is aligned to.

        // Rotation-related variables
        [Header("Rotation")]
        [SerializeField] Vector2 defaultRotation; // Default rotation of the camera when aiming.
        [SerializeField] Vector2 lookAngleBounds;
        [SerializeField] float lookSensitivity; // Sensitivity of the camera's rotation to player input.
        Quaternion rotation = Quaternion.identity; // Current rotation of the camera.
        Vector2 lookRotation; // Accumulated rotation values (X for yaw, Y for pitch).
        Vector2 look; // Input-based rotation delta.

       

        // Offset-related variables
        [Header("Offset")]
        [SerializeField] float heightOffset; // Vertical offset of the camera from the player.

        // Called when the script is initialized
        void Awake()
        {
            distance = defaultDistance; // Initialize the camera distance to the default value.
        }

        // Called every frame to handle input and update camera state
        void Update()
        {
            look += inputHandler.lookDelta; // Accumulate rotation input from the player.
            distance = CalculateDistance(inputHandler.zoomDelta.value); // Update the target distance based on zoom input.
        }

        // Determines if this camera state should be active
        public override bool Check()
        {
            return inputHandler.aim.hold; // Activate this state if the player is holding the "roll" input (used for aiming).
        }

        // Main logic for updating the camera's position and rotation
        public override void Execute()
        {
            // Calculate the player's model position with the offset
            Vector3 modelPosition = modelTransform.position + (modelTransform.rotation * positionOffset);

            // Input handling
            if (transition <= 0)
                look += inputHandler.look / Time.fixedDeltaTime; // Accumulate input for smooth rotation.

            distance = CalculateDistance(inputHandler.zoom.value / Time.fixedDeltaTime); // Update the target distance based on zoom input.

            // Zoom logic
            currentDistance = Mathf.Lerp(currentDistance, distance, 1 - zoomSmoothness); // Smoothly interpolate the camera's distance.

            // Normal alignment logic
            normal = Vector3.Lerp(normal,
                groundInformation.ground ? groundInformation.hit.normal : Vector3.up, // Align with ground normal or default to upward direction.
                1 - normalSmoothness);

            // Rotation logic
            lookRotation += look * lookSensitivity; // Apply input-based rotation.
            lookRotation.y = Mathf.Clamp(lookRotation.y, lookAngleBounds.x, lookAngleBounds.y); // Clamp the pitch to specified bounds.
            look = Vector2.zero; // Reset input after applying it.

            // Calculate forward, right, and up vectors for the camera based on rotation
            Vector3 forward = Quaternion.AngleAxis(lookRotation.x, Vector3.up) * Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 up = Quaternion.AngleAxis(-lookRotation.y, right) * Vector3.up;
            forward = Vector3.ProjectOnPlane(forward, up).normalized;

            // Calculate the camera's goal rotation based on the aiming direction
            Quaternion goalRotation = Quaternion.LookRotation(forward, up);

            // Position logic
            Vector3 goalPosition = modelPosition
                + (goalRotation * -Vector3.forward * currentDistance) // Position the camera behind the player
                + (normal * heightOffset); // Apply height offset

            // Apply the calculated position and rotation to the camera
            //cameraTransform.position = Vector3.Lerp(cameraTransform.position, goalPosition, 1 - zoomSmoothness);
            //cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, goalRotation, 1 - zoomSmoothness);
            cameraTransform.position = modelPosition
                + Vector3.Slerp(oldPosition,
                    goalPosition - modelPosition,
                    Mathf.SmoothStep(1, 0, transition / transitionTime));

            cameraTransform.rotation = Quaternion.Slerp(oldRotation,
                goalRotation,
                1 - Mathf.Sin(Mathf.SmoothStep(0, 1, transition / transitionTime) * Mathf.PI / 2));
        }


        // Called when this state is enabled
        public override void Enable()
        {
            look = Vector2.zero; // Reset look input.
            lookRotation = defaultRotation; // Reset rotation to default.
            normal = groundInformation.ground ? groundInformation.hit.normal : Vector3.up; // Align with ground normal or default to upward direction.
            rotation = cameraTransform.rotation; // Set initial rotation.
            distance = defaultDistance; // Reset distance to default.
            currentDistance = distance; // Initialize smoothed distance.
        }

        // Handles transitions into this state
        public override void Transition()
        {
            base.Transition(); // Call base transition logic.
            Enable(); // Reinitialize state variables.
            rotation = Quaternion.identity; // Reset rotation.
            lookRotation.x = Vector3.SignedAngle(modelTransform.forward, Vector3.forward, -Vector3.up); // Align rotation with the player's forward direction.
        }

        // Calculates the target zoom distance based on input
        float CalculateDistance(float delta)
        {
            return Mathf.Clamp(distance
                + (delta
                    * zoomSensitivity
                    * Mathf.Clamp01(Mathf.Max((maxDistance - distance) / zoomFalloff, -Mathf.Sign(delta * zoomSensitivity)))
                    * Mathf.Clamp01(Mathf.Max((distance - minDistance) / zoomFalloff, Mathf.Sign(delta * zoomSensitivity)))),
                minDistance,
                maxDistance);
        }
    }
}
