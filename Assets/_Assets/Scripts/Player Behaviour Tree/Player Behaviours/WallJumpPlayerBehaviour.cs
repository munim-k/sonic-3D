using UnityEngine;

namespace RagdollEngine
{
    public class WallJumpPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float wallJumpCooldown; // Cooldown time between wall jumps.
        [SerializeField] float wallJumpForce; // Force applied during a wall jump.
        [SerializeField] float wallJumpSpeed;
        [SerializeField] LayerMask wallLayerMask; // Layer mask to identify walls.

        float wallJumpCooldownTimer = 0;
        bool wallJumping; // Indicates if the player is currently wall jumping.
        Vector3 wallNormal; // Stores the normal of the wall the player is in contact with.
        Vector3 goalPosition; // The target position the player moves toward during the wall jump.
        Vector3 origPos;
        float walljumpLerp = 0f;
        public override bool Evaluate()
        {
            if (wallJumpCooldownTimer > 0)
            {
                wallJumpCooldownTimer -= Time.deltaTime;
            }
            // Check if the player is in contact with a wall.
            //Do raycasts in all 4 directions and use the first one to hit
            if (!wallJumping)
            {
                bool wallContact = false;


                wallContact = Physics.Raycast(
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
                if (wallContact)
                {
                    // Store the wall normal and reset the wall jump timer.
                    wallNormal = hit.normal;
                    if (inputHandler.jump.pressed && wallJumpCooldownTimer <= 0)
                    {
                        StartWallJump();
                    }
                }
            }
            else
            {
                PerformWallJump();
            }
            //If the player is on the ground
            if (groundInformation.ground)
            {
                wallJumping = false; // Exit wall jump state.
                kinematic = false; // Exit kinematic mode.
                overrideModelTransform = false; // Allow other behaviors to control the model transform.
            }
            return wallJumping;
        }
        private void StartWallJump()
        {
            // Calculate the goal position for the wall jump.
            origPos=playerTransform.position;
            goalPosition = playerTransform.position + (wallNormal + Vector3.up).normalized * wallJumpForce;
            //Raycast towards the goal position and shorten it if necessary
           if(Physics.Raycast(playerTransform.position, goalPosition - playerTransform.position, out RaycastHit hit, (goalPosition - playerTransform.position).magnitude, wallLayerMask, QueryTriggerInteraction.Ignore))
            {
                goalPosition = hit.point;
            }

            // Trigger jump animation.
            animator.SetTrigger("Jump");

            // Set the player to kinematic mode.
            walljumpLerp = 0f;
            kinematic = true;
            wallJumping = true;
            overrideModelTransform = true; // Override the model transform to control the player's position.
            wallJumpCooldownTimer = wallJumpCooldown;
        }

        private void PerformWallJump()
        {
            // Move the player toward the goal position.
            Vector3 difference = goalPosition - playerTransform.position;


            // If the player has reached the goal position, end the wall jump.
            if (difference.magnitude <= 0.1f)
            {
                walljumpLerp = 0f; // Reset the lerp value.
                wallJumping = false;
                kinematic = false; // Exit kinematic mode.
                overrideModelTransform = false; // Allow other behaviors to control the model transform.
                return;
            }


            // Update the player's position to move toward the goal
            walljumpLerp += Time.fixedDeltaTime * wallJumpSpeed;
            playerTransform.position = Vector3.Lerp(origPos, goalPosition, walljumpLerp); // Smooth movement.
            playerTransform.rotation = Quaternion.LookRotation(difference); // Rotate the model to face the goal position.
            Debug.DrawLine(origPos, goalPosition, Color.red); // Draw a line in the editor for visualization.
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(goalPosition, 0.5f); // Draw a sphere at the goal position for visualization.
        }
    }
}
