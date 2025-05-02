using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class PumpkinDogMovePlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] float maxSpeed; // Maximum speed the player can move
        [SerializeField] float baseSpeed; // Base speed when starting movement
        [SerializeField] float acceleration; // Acceleration rate when moving
        [SerializeField, Range(0, 1)] float smoothness; // Smoothness factor for movement transitions
        [SerializeField] float uphillSlopeRatio; // Speed reduction factor for uphill slopes
        [SerializeField] float downhillSlopeRatio; // Speed increase factor for downhill slopes
        [SerializeField] float maxSpeedUphillSlopeRatio; // Maximum speed reduction on steep uphill slopes
        [SerializeField] float maxSpeedDownhillSlopeRatio; // Maximum speed increase on steep downhill slopes

        bool wasMoving; // Tracks whether the player was moving in the previous frame

        public override void Execute()
        {
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

            if (moving)
            {
                // Adjust movement direction based on the terrain plane
                if (plane.magnitude > 0)
                {
                    Vector3 axis = Vector3.Cross(plane, playerTransform.up);
                    moveNormal = axis.normalized * Mathf.Sign(Vector3.Dot(moveNormal, axis)) * moveNormal.magnitude;
                }
                moveVelocity = moveNormal * maxSpeed;
            }
            else
            {
                moveVelocity = Vector3.zero;
            }
            Vector3 finalVel = moveVelocity - RB.linearVelocity;
            finalVel.y = 0;
            additiveVelocity += finalVel;

                //// Adjust for slopes if the player is on a slope
                //if (groundInformation.slope)
                //{
                //    accelerationVector -= Vector3.ProjectOnPlane(groundInformation.hit.normal, playerTransform.up).normalized
                //        * Mathf.Min(Vector3.Dot(accelerationVector, Vector3.ProjectOnPlane(groundInformation.hit.normal, playerTransform.up).normalized), 0);
                //}

                //// Apply additional velocity adjustments for uphill or downhill movement
                //if (moving || moveVelocity.magnitude > moveDeadzone)
                //{
                //    additiveVelocity += Vector3.Project(
                //        -Vector3.up * (Vector3.Dot(moveVelocity, Vector3.up) >= 0
                //            ? Mathf.Lerp(uphillSlopeRatio, maxSpeedUphillSlopeRatio, speedPercent)
                //            : Mathf.Lerp(downhillSlopeRatio, maxSpeedDownhillSlopeRatio, speedPercent)),
                //        moveVelocity.normalized);
                //}

                // Update the movement state for the next frame
                wasMoving = moving;
        }
    }
}
