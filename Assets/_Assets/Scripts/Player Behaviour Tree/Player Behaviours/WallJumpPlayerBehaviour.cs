using UnityEngine;

namespace RagdollEngine
{
    public class WallJumpPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float wallJumpCooldown; // Cooldown time between wall jumps.
        [SerializeField] float wallJumpForceHorizontal; // Force applied during a wall jump.
        [SerializeField] float wallJumpForceVertical; // Force applied during a wall jump.
        [SerializeField] float wallJumpTime;
        [SerializeField] float wallJumpDecreaseSpeed;
        [SerializeField] LayerMask wallLayerMask; // Layer mask to identify walls.

        float speed = 0;
        private float wallJumpCooldownTimer; // Timer for the wall jump cooldown.
        Vector3 goalPosition; // The target position the player moves toward during the wall jump.
        Vector3 jumpVector;
        Vector3 wallPoint;
        Vector3 wallNormal;
        private int prevWallID = -1; // ID of the previous wall hit by the player.
        RaycastHit hit;

        // Tracks the current length of the spring
        float currentLength;

        // Speed of the springing motion


        //public override bool Evaluate()
        //{
        //    if (wallJumpCooldownTimer > 0)
        //    {
        //        wallJumpCooldownTimer -= Time.deltaTime;
        //    }
        //    // Check if the player is in contact with a wall.
        //    //Do raycasts in all 4 directions and use the first one to hit
        //    if (!wallJumping)
        //    {
        //        bool wallContact = false;
        //        wallContact = Physics.Raycast(
        //           playerTransform.position, // Start position of the raycast.
        //           playerTransform.forward, // Direction of the raycast (forward).
        //           out RaycastHit hit, // Stores information about the object hit by the raycast.
        //           height + Physics.defaultContactOffset, // Raycast distance.
        //           wallLayerMask, // Layer mask to filter walls.
        //           QueryTriggerInteraction.Ignore // Ignore trigger colliders.
        //       );
        //        if (!wallContact)
        //        {
        //            wallContact = Physics.Raycast(
        //                playerTransform.position, // Start position of the raycast.
        //                -playerTransform.forward, // Direction of the raycast (backward).
        //                out hit, // Stores information about the object hit by the raycast.
        //                height + Physics.defaultContactOffset, // Raycast distance.
        //                wallLayerMask, // Layer mask to filter walls.
        //                QueryTriggerInteraction.Ignore // Ignore trigger colliders.
        //            );
        //        }
        //        if (!wallContact)
        //        {
        //            wallContact = Physics.Raycast(
        //                playerTransform.position, // Start position of the raycast.
        //                playerTransform.right, // Direction of the raycast (right).
        //                out hit, // Stores information about the object hit by the raycast.
        //                height + Physics.defaultContactOffset, // Raycast distance.
        //                wallLayerMask, // Layer mask to filter walls.
        //                QueryTriggerInteraction.Ignore // Ignore trigger colliders.
        //            );
        //        }
        //        if (!wallContact)
        //        {
        //            wallContact = Physics.Raycast(
        //                playerTransform.position, // Start position of the raycast.
        //                -playerTransform.right, // Direction of the raycast (left).
        //                out hit, // Stores information about the object hit by the raycast.
        //                height + Physics.defaultContactOffset, // Raycast distance.
        //                wallLayerMask, // Layer mask to filter walls.
        //                QueryTriggerInteraction.Ignore // Ignore trigger colliders.
        //            );
        //        }
        //        if (wallContact)
        //        {
        //            // Store the wall normal and reset the wall jump timer.
        //            wallNormal = hit.normal;
        //            if (inputHandler.jump.pressed && wallJumpCooldownTimer <= 0)
        //            {
        //                StartWallJump();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        PerformWallJump();
        //    }
        //    //If the player is on the ground
        //    if (groundInformation.ground)
        //    {
        //        wallJumping = false; // Exit wall jump state.
        //        kinematic = false; // Exit kinematic mode.
        //        overrideModelTransform = false; // Allow other behaviors to control the model transform.
        //    }
        //    return wallJumping;
        //}
        //private void StartWallJump()
        //{
        //    // Calculate the goal position for the wall jump.
        //    origPos = playerTransform.position;
        //    goalPosition = playerTransform.position + (wallNormal + Vector3.up).normalized * wallJumpForce;
        //    jumpVelocity = wallNormal * wallJumpForce * jumpVelocityMul;

        //    //Raycast towards the goal position and shorten it if necessary
        //    if (Physics.Raycast(playerTransform.position, goalPosition - playerTransform.position, out RaycastHit hit, (goalPosition - playerTransform.position).magnitude, wallLayerMask, QueryTriggerInteraction.Ignore))
        //    {
        //        goalPosition = hit.point;
        //    }

        //    // Trigger jump animation.
        //    animator.SetTrigger("Jump");

        //    // Set the player to kinematic mode.
        //    walljumpLerp = 0f;
        //    kinematic = true;
        //    wallJumping = true;
        //    overrideModelTransform = true; // Override the model transform to control the player's position.
        //    wallJumpCooldownTimer = wallJumpCooldown;
        //}

        //private void PerformWallJump()
        //{
        //    // Move the player toward the goal position.
        //    Vector3 difference = goalPosition - playerTransform.position;


        //    // If the player has reached the goal position, end the wall jump.
        //    if (difference.magnitude <= 0.1f)
        //    {
        //        walljumpLerp = 0f; // Reset the lerp value.
        //        wallJumping = false;
        //        kinematic = false; // Exit kinematic mode.
        //        overrideModelTransform = false; // Allow other behaviors to control the model transform.
        //        additiveVelocity += jumpVelocity;
        //        return;
        //    }


        //    // Update the player's position to move toward the goal
        //    walljumpLerp += Time.fixedDeltaTime * wallJumpSpeed;
        //    playerTransform.position = Vector3.Lerp(origPos, goalPosition, walljumpLerp); // Smooth movement.
        //    playerTransform.rotation = Quaternion.LookRotation(difference); // Rotate the model to face the goal position.
        //    Debug.DrawLine(origPos, goalPosition, Color.red); // Draw a line in the editor for visualization.
        //}

        public override bool Evaluate()
        {
            // Check if the player can interact with a spring
            if (!WallCheck()) return false;

            // Calculate the goal position for the player based on the spring's position and length
            Vector3 goal = wallPoint +
                (jumpVector * (wallJumpTime - currentLength));

            // Set the player's target position
            movePosition = goal;

            // Make the player kinematic (disable physics-based movement)
            kinematic = true;

            // Align the player's model to face the wall's direction
            modelTransform.rotation = Quaternion.LookRotation(jumpVector, modelTransform.up);

            // Position the player's model at the calculated goal position
            modelTransform.position = goal - (modelTransform.up * height);

            // Override the model's transform to prevent other behaviors from modifying it
            overrideModelTransform = true;

            // Reduce the current length of the spring based on speed and velocity
            currentLength = Mathf.Max(currentLength - speed, 0);

            // If the spring has fully compressed, deactivate the behavior
            if (currentLength <= 0)
                return false;
            return true; // The behavior remains active
        }

        // Checks if the player can interact with a Wall
        bool WallCheck()
        {
            // Iterate through all stage objects in the scene

            // Check if the stage object is a spring
            if (wallJumpCooldownTimer >= 0)
            {
                wallJumpCooldownTimer -= Time.fixedDeltaTime;

            }
            else
            {
                if (inputHandler.jump.pressed)
                {

                    bool wallContact = false;
                    wallContact = Physics.Raycast(
                       playerTransform.position, // Start position of the raycast.
                       playerTransform.forward, // Direction of the raycast (forward).
                       out hit, // Stores information about the object hit by the raycast.
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
                        int wallID = hit.collider.gameObject.GetInstanceID();
                        if (prevWallID != wallID)
                        {
                            prevWallID = wallID;
                            // Set the current length of the spring
                            currentLength = wallJumpTime;
                            speed = wallJumpDecreaseSpeed;
                            // Calculate the additive velocity to apply to the player
                            wallPoint = hit.point;
                            wallNormal = hit.normal;
                            jumpVector = wallNormal.normalized * wallJumpForceHorizontal;
                            jumpVector += Vector3.up * wallJumpForceVertical;
                            additiveVelocity = -RB.linearVelocity + jumpVector;
                            goalPosition = playerTransform.position + jumpVector;
                            wallJumpCooldownTimer = wallJumpCooldown;
                            return true; // The player can interact with the wall
                        }
                    }
                }

            }
            // If the player was previously active and the player is still jumping, return true
            if (wasActive && currentLength > 0) return true;

            return false; // The player cannot interact with a wall
        }
    }
}
