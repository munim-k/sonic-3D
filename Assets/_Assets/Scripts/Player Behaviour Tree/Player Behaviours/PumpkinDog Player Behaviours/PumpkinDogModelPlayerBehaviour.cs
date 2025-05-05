using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class PumpkinDogModelPlayerBehaviour : PlayerBehaviour
    {
        // Serialized fields for tuning movement and rotation behavior
        [SerializeField] float groundSmoothness; // Smoothness of model alignment when grounded
        [SerializeField] float airSmoothness;    // Smoothness of model alignment when airborne
        [SerializeField] float maxSpeed;         // Maximum speed of the player

        public override void Execute()
        {
            
            // Calculate the player's current speed based on input or velocity
            float speed = inputHandler.move.magnitude > InputSystem.settings.defaultDeadzoneMin || moveVelocity.magnitude > moveDeadzone
                ? moveVelocity.magnitude
                : 0;
            // Update the animator's "Moving" parameter to indicate whether the player is moving
            animator.SetBool("Moving", speed > 0);

            // Update the animator's "Ground" parameter to indicate whether the player is grounded
            animator.SetBool("Ground", groundInformation.ground);

            if (groundInformation.ground)
            {
                // Update animator parameters related to speed when the player is grounded
                animator.SetFloat("Speed", speed); // Current speed
                animator.SetFloat("World Speed", RB.linearVelocity.magnitude); // World-relative speed
                animator.SetFloat("Speed Percent", Mathf.Clamp01(speed / maxSpeed)); // Speed as a percentage of max speed
            }

            // Update the animator's "Vertical Velocity" parameter to reflect the player's vertical movement
            animator.SetFloat("Vertical Velocity", Vector3.Dot(RB.linearVelocity, playerTransform.up));



            // Smoothly align the model's "up" direction with the player's "up" direction
            Vector3 up = Vector3.Lerp(
                modelTransform.up,
                playerTransform.up,
                1 - (groundInformation.ground ? groundSmoothness : airSmoothness)
            );

            // If the model's transform is overridden, exit early
            if (overrideModelTransform) return ;

            // Update the model's rotation to align with the calculated forward and up directions

            modelTransform.rotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(cameraTransform.forward - Vector3.Project(cameraTransform.forward, plane), up), up);
            // Update the model's position to match the ground or player's position
            modelTransform.position = groundInformation.cast
                ? groundInformation.hit.point
                : playerTransform.position - (modelTransform.up * height);

        }
    }
}
