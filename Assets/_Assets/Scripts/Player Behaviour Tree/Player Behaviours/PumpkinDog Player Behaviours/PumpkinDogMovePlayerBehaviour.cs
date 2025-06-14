using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine {
    public class PumpkinDogMovePlayerBehaviour : PlayerBehaviour {
        [SerializeField] float maxSpeed; // Maximum speed the player can move
        [SerializeField] float platformSpeed; // Speed of the moving platform
        private Transform currentPlatform;
        private Vector3 previousPlatformPosition;

        bool wasMoving; // Tracks whether the player was moving in the previous frame

        public override void Execute() {
            active = true; // Mark this behavior as active

            // Determine if the player was moving in the previous frame and is still active
            wasMoving = wasMoving && wasActive;

            // Check if the player is providing movement input
            moving = inputHandler.move.magnitude > InputSystem.settings.defaultDeadzoneMin;

            // Calculate movement directions relative to the camera and player orientation
            Vector3 moveForwardNormal = Vector3.Cross(cameraTransform.right, playerTransform.up).normalized; // Forward direction
            Vector3 moveRightNormal = Vector3.Cross(playerTransform.up, cameraTransform.forward).normalized; // Right direction

            // Combine forward and right movement based on input
            Vector3 moveNormal = (moveForwardNormal
                    * inputHandler.move.y
                    * Mathf.Sign(Vector3.Dot(tangent, moveForwardNormal)
                        * Mathf.Max(Vector3.Dot(cameraTransform.forward, tangent),
                            -Vector3.Dot(Vector3.Cross(tangent, plane), cameraTransform.up))))
                + (moveRightNormal
                    * inputHandler.move.x
                    * Mathf.Sign(Vector3.Dot(tangent, moveRightNormal)
                        * Mathf.Max(Vector3.Dot(cameraTransform.forward, -plane),
                            -Vector3.Dot(Vector3.Cross(tangent, plane), cameraTransform.up))));

            // Calculate the percentage of the maximum speed the player is currently moving at
            float speedPercent = Mathf.Min(moveVelocity.magnitude / maxSpeed, 1);

            if (moving) {
                // Adjust movement direction based on the terrain plane
                if (plane.magnitude > 0) {
                    Vector3 axis = Vector3.Cross(plane, playerTransform.up);
                    moveNormal = axis.normalized * Mathf.Sign(Vector3.Dot(moveNormal, axis)) * moveNormal.magnitude;
                }
                moveVelocity = moveNormal * maxSpeed;
            }
            else {
                moveVelocity = Vector3.zero;
            }
            Vector3 finalVel = moveVelocity - RB.linearVelocity;
            finalVel.y = 0;
            additiveVelocity += finalVel;
            // Update the movement state for the next frame

            if (groundInformation.ground && groundInformation.hit.collider.CompareTag("MovingPlatform")) {
                Transform platformTransform = groundInformation.hit.collider.transform;

                // If the platform has changed, reset the previous position
                if (currentPlatform != platformTransform) {
                    currentPlatform = platformTransform;
                    previousPlatformPosition = platformTransform.position;
                }

                // Calculate the platform's velocity based on its position change
                Vector3 platformVelocity = (platformTransform.position - previousPlatformPosition) / Time.fixedDeltaTime;

                // Update the previous position for the next frame
                previousPlatformPosition = platformTransform.position;
                platformVelocity *= platformSpeed;
                // Add the platform's velocity to the player's additive velocity
                additiveVelocity += platformVelocity;
            }
            else {
                // Reset platform tracking if the player is no longer on a moving platform
                currentPlatform = null;
            }
            wasMoving = moving;
        }
    }
}
