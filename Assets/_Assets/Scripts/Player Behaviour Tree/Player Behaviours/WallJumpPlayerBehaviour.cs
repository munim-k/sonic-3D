using UnityEngine;

namespace RagdollEngine
{
    public class WallJumpPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] int maxJumps; //Maximum number of consecutive jumps allowed
        [SerializeField] float wallJumpCooldown; // Cooldown time between wall jumps.
        [SerializeField] float wallJumpForce; // Force applied during a wall jump.
        [SerializeField] float wallJumpSpeed;
        [SerializeField] LayerMask wallLayerMask; // Layer mask to identify walls.
       
        int currentWallJumps = 0;
        float wallJumpCooldownTimer = 0;
        bool wallJumping; // Indicates if the player is currently wall jumping.
        Vector3 wallNormal; // Stores the normal of the wall the player is in contact with.
        Vector3 goalPosition; // The target position the player moves toward during the wall jump.

        public override bool Evaluate()
        {
            if (wallJumpCooldownTimer > 0)
            {
                wallJumpCooldownTimer -= Time.deltaTime;
            }
            // Check if the player is in contact with a wall.
            //Do raycasts in all 4 directions and use the first one to hit

            bool wallContact = Physics.Raycast(
                playerTransform.position, // Start position of the raycast.
                playerTransform.forward, // Direction of the raycast (forward).
                out RaycastHit hit, // Stores information about the object hit by the raycast.
                height + Physics.defaultContactOffset, // Raycast distance.
                wallLayerMask, // Layer mask to filter walls.
                QueryTriggerInteraction.Ignore // Ignore trigger colliders.
            );
            if (!wallContact)
            {
                wallContact = Physics.Raycast(
                    playerTransform.position, // Start position of the raycast.
                    -playerTransform.forward, // Direction of the raycast (backward).
                    out hit, // Stores information about the object hit by the raycast.
                    height + Physics.defaultContactOffset, // Raycast distance.
                    wallLayerMask, // Layer mask to filter walls.
                    QueryTriggerInteraction.Ignore // Ignore trigger colliders.
                );
            }
            if (!wallContact)
            {
                wallContact = Physics.Raycast(
                    playerTransform.position, // Start position of the raycast.
                    playerTransform.right, // Direction of the raycast (right).
                    out hit, // Stores information about the object hit by the raycast.
                    height + Physics.defaultContactOffset, // Raycast distance.
                    wallLayerMask, // Layer mask to filter walls.
                    QueryTriggerInteraction.Ignore // Ignore trigger colliders.
                );
            }
            if (!wallContact)
            {
                wallContact = Physics.Raycast(
                    playerTransform.position, // Start position of the raycast.
                    -playerTransform.right, // Direction of the raycast (left).
                    out hit, // Stores information about the object hit by the raycast.
                    height + Physics.defaultContactOffset, // Raycast distance.
                    wallLayerMask, // Layer mask to filter walls.
                    QueryTriggerInteraction.Ignore // Ignore trigger colliders.
                );
            }
            //If the player is contacting a wall
            if (wallContact)
            {
                // Store the wall normal and reset the wall jump timer.
                wallNormal = hit.normal;
                if (inputHandler.jump.pressed && wallJumpCooldownTimer <= 0 && currentWallJumps < maxJumps)
                {
                    StartWallJump();
                }
            }
            // If the player is in the middle of a wall jump, move toward the goal position.
            if (wallJumping)
            {
                PerformWallJump();
            }
            //If the player is on the ground
            if (groundInformation.ground)
            {
                // Reset the wall jump counter.
                currentWallJumps = 0;
                wallJumping = false; // Exit wall jump state.
                kinematic = false; // Exit kinematic mode.
                overrideModelTransform = false; // Allow other behaviors to control the model transform.
            }
            return wallJumping;
        }
        private void StartWallJump()
        {
            // Calculate the goal position for the wall jump.
            goalPosition = playerTransform.position + (wallNormal + Vector3.up).normalized * wallJumpForce;


            // Trigger jump animation.
            animator.SetTrigger("Jump");

            // Set the player to kinematic mode.
            kinematic = true;
            wallJumping = true;
            overrideModelTransform = true; // Override the model transform to control the player's position.
            currentWallJumps++;
            wallJumpCooldownTimer = wallJumpCooldown;
        }

        private void PerformWallJump()
        {
            // Move the player toward the goal position.
            Vector3 difference = goalPosition - playerTransform.position;


            // If the player has reached the goal position, end the wall jump.
            if (difference.magnitude <= 0.1f)
            {
                wallJumping = false;
                kinematic = false; // Exit kinematic mode.
                overrideModelTransform = false; // Allow other behaviors to control the model transform.
                return;
            }


            // Update the player's position to move toward the goal.
            movePosition = Vector3.Lerp(playerTransform.position, goalPosition, Time.fixedDeltaTime * wallJumpSpeed); // Smooth movement.
        }
    }
}
