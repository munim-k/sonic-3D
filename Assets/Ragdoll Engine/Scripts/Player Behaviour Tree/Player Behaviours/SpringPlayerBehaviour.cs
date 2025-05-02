using UnityEngine;

namespace RagdollEngine
{
    public class SpringPlayerBehaviour : PlayerBehaviour
    {
        // Reference to the spring stage object the player interacts with
        SpringStageObject springStageObject;

        // Indicates whether the player is currently springing
        bool spring;

        // Tracks the current length of the spring
        float currentLength;

        // Speed of the springing motion
        float speed;

        // Called every frame after Update
        void LateUpdate()
        {
            // Determine if the player is springing
            spring = active // If the behavior is active
                || (spring // Or if the player was already springing
                    && !groundInformation.ground // And the player is not on the ground
                    && RB.linearVelocity.y >= 0); // And the player is moving upwards

            // Update the animator to reflect the springing state
            animator.SetBool("Springing", spring);
        }

        // Evaluates whether the spring behavior should be active
        public override bool Evaluate()
        {
            // Check if the player can interact with a spring
            if (!SpringCheck()) return false;

            // Calculate the goal position for the player based on the spring's position and length
            Vector3 goal = springStageObject.transform.position +
                (springStageObject.transform.up * (springStageObject.length - currentLength));

            // Set the player's target position
            movePosition = goal;

            // Make the player kinematic (disable physics-based movement)
            kinematic = true;

            // Align the player's model to face the spring's direction
            modelTransform.rotation = Quaternion.LookRotation(springStageObject.transform.forward, springStageObject.transform.up);

            // Position the player's model at the calculated goal position
            modelTransform.position = goal - (modelTransform.up * height);

            // Override the model's transform to prevent other behaviors from modifying it
            overrideModelTransform = true;

            // Reduce the current length of the spring based on speed and velocity
            currentLength = Mathf.Max(
                currentLength - (Mathf.Lerp(
                    springStageObject.speed,
                    speed,
                    Vector3.Dot(RB.linearVelocity, springStageObject.transform.up) > 0 ? RB.linearVelocity.magnitude : 0
                ) * Time.fixedDeltaTime),
                0
            );

            // If the spring has fully compressed, deactivate the behavior
            if (currentLength <= 0)
                return false;

            return true; // The behavior remains active
        }

        // Checks if the player can interact with a spring
        bool SpringCheck()
        {
            // Iterate through all stage objects in the scene
            foreach (StageObject thisStageObject in stageObjects)
            {
                // Check if the stage object is a spring
                if (thisStageObject is SpringStageObject)
                {
                    // If the player was already interacting with this spring, return true
                    if (wasActive && thisStageObject == springStageObject) return true;

                    // Set the current spring stage object
                    springStageObject = thisStageObject as SpringStageObject;

                    // Play the spring's audio effect
                    springStageObject.audioSource.Play();

                    // Calculate the goal position for the player to align with the spring
                    Vector3 goal = playerTransform.position -
                        Vector3.ProjectOnPlane(playerTransform.position - thisStageObject.transform.position, thisStageObject.transform.up);

                    // Calculate the difference between the player's position and the goal
                    Vector3 difference = playerTransform.position - goal;

                    // If there are no obstacles between the player and the goal, move the player to the goal
                    if (!Physics.Raycast(playerTransform.position, difference.normalized, difference.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                        playerTransform.position = goal;

                    // Set the current length of the spring
                    currentLength = springStageObject.length;

                    // Determine the speed of the spring based on the player's velocity
                    speed = wasActive
                        ? Mathf.Max(springStageObject.speed, speed) // Use the higher of the spring's speed or the current speed
                        : Vector3.Dot(RB.linearVelocity, springStageObject.transform.up) > 0
                            ? Mathf.Max(springStageObject.speed, RB.linearVelocity.magnitude) // Use the higher of the spring's speed or the player's velocity
                            : springStageObject.speed;

                    // Calculate the additive velocity to apply to the player
                    additiveVelocity = -RB.linearVelocity
                        + thisStageObject.transform.up *
                        (Vector3.Dot(RB.linearVelocity, thisStageObject.transform.up) > 0
                            ? Mathf.Max(springStageObject.speed, RB.linearVelocity.magnitude)
                            : springStageObject.speed);

                    // Uncomment this line to trigger a spring animation
                    // animator.SetTrigger("Spring");

                    return true; // The player can interact with the spring
                }
            }

            // If the player was previously active and the spring is still compressing, return true
            if (wasActive && currentLength > 0) return true;

            return false; // The player cannot interact with a spring
        }
    }
}
