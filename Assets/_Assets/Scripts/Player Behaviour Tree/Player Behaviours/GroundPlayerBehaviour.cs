using UnityEngine;

namespace RagdollEngine
{
    public class GroundPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float groundDistance; // Distance to check for ground below the player.
        [SerializeField] float slopeLimit; // Maximum slope angle the player can stand on.
        [SerializeField] float slopeCooldownLimit; // Slope angle limit during slope cooldown.
        [SerializeField] float localSlopeLimit; // Maximum local slope angle relative to the player.
        [SerializeField] float slopeCooldownTime; // Time duration for slope cooldown.
        [SerializeField] float loopSpeed; // Speed threshold for slope adjustments.
        [SerializeField] float platformSpeed; // Speed threshold for platform adjustments.
       

        bool slopeCooldown; // Indicates if the player is in slope cooldown.
        bool initialized; // Tracks if the behavior has been initialized.

        // This method evaluates whether the player is on the ground.
        public override bool Evaluate()
        {
            // Initialize the behavior if it hasn't been initialized yet.
            if (!initialized)
            {
                initialized = true;
                Execute(); // Perform the ground check immediately upon initialization.
            }

            // Return whether the player is on the ground.
            return groundInformation.ground;
        }

        // This method performs the main logic for detecting ground and handling slopes.
        public override void Execute()
        {
            // Perform a raycast to check for ground below the player.
            bool cast = Physics.Raycast(
                playerTransform.position, // Start position of the raycast.
                -playerTransform.up, // Direction of the raycast (downward).
                out RaycastHit hit, // Stores information about the object hit by the raycast.
                (groundInformation.ground ? height + groundDistance : height) // Raycast distance.
                    + Mathf.Max(Vector3.Dot(RB.linearVelocity, -playerTransform.up) * Time.fixedDeltaTime, 0) // Adjust for velocity.
                    + Physics.defaultContactOffset, // Add a small offset to avoid precision issues.
                layerMask, // Layer mask to filter which objects the raycast interacts with.
                QueryTriggerInteraction.Ignore // Ignore trigger colliders.
            );

            // Determine if the surface hit by the raycast is too steep.
            bool slope = cast
                && (Vector3.Angle(hit.normal, Vector3.up) > (slopeCooldown ? slopeCooldownLimit : Mathf.Lerp(slopeLimit, 180, Mathf.Clamp(RB.linearVelocity.magnitude / (Vector3.Dot(hit.normal, Vector3.up) >= 0 ? moveDeadzone : loopSpeed), 0, 1)))
                    || Vector3.Angle(hit.normal, playerTransform.up) > localSlopeLimit);

            // If the surface is too steep, enable slope cooldown.
            if (cast && slope)
                slopeCooldown = true;

            // Determine if the player is on valid ground.
            bool ground = cast
                && !slope // Ensure the surface is not too steep.
                && (groundInformation.ground || Vector3.Dot(RB.linearVelocity, hit.normal) <= 0); // Ensure the player is not moving away from the ground.

            // If the player is on valid ground, disable slope cooldown.
            if (ground && slopeCooldown)
                slopeCooldown = false;

            // Update the ground information with the results of the raycast and calculations.
            groundInformation = new PlayerBehaviourTree.GroundInformation()
            {
                hit = hit, // Store the raycast hit information.
                ground = ground, // Whether the player is on valid ground.
                cast = cast, // Whether the raycast hit something.
                slope = slope, // Whether the surface is too steep.
                enter = ground && !wasActive // Whether the player just entered valid ground.
            };
            // Handle moving platform logic.
           
            // Update the active state of the behavior based on whether the player is on valid ground.
            active = ground;
        }
    }
}
