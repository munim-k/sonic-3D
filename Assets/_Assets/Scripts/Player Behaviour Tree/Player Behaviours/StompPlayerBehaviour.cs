using UnityEngine;

namespace RagdollEngine
{
    public class StompPlayerBehaviour : PlayerBehaviour
    {
        // Audio source for playing landing sound
        [SerializeField] AudioSource landAudioSource;

        // Minimum force applied during a stomp
        [SerializeField] float minStompForce;

        // Maximum force applied during a stomp
        [SerializeField] float maxStompForce;

        // Time it takes to reach maximum stomp force
        [SerializeField] float stompAccelerationTime;

        // Timer to track the remaining acceleration time
        float stompAccelerationTimer;

        // Called once per frame, after all Update calls
        void LateUpdate()
        {
            // Update the animator's "Stomping" state based on whether the stomp is active
            animator.SetBool("Stomping", active);

            // Play landing sound if the player was stomping and has landed on the ground
            if (wasActive && groundInformation.ground)
                landAudioSource.Play();
        }

        // Evaluates whether the stomp behavior should be active
        public override bool Evaluate()
        {
            // Check if the stomp button is pressed or if the player is already stomping and not grounded
            bool stomping = (inputHandler.stomp.pressed || wasActive) && !groundInformation.ground;

            if (stomping)
            {
                if (wasActive)
                {
                    // Decrease the acceleration timer if the stomp is ongoing
                    stompAccelerationTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    // Reset the acceleration timer and trigger the stomp animation
                    stompAccelerationTimer = stompAccelerationTime;
                    animator.SetTrigger("Stomp");
                }
            }

            return stomping;
        }

        // Executes the stomp behavior by applying downward velocity
        public override void Execute()
        {
            // Calculate the downward velocity based on the stomp force and remaining acceleration time
            additiveVelocity = -RB.linearVelocity
                + (-Vector3.up * Mathf.Lerp(
                    minStompForce,
                    maxStompForce,
                    1 - Mathf.Pow(10, -(1 - (stompAccelerationTimer / stompAccelerationTime)))
                ));
        }
    }
}