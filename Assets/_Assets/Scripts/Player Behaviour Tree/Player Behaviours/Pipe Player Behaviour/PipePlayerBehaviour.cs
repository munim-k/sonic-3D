using RagdollEngine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
namespace RagdollEngine {
    public class PipePlayerBehaviour : PlayerBehaviour
{
        [SerializeField] private LayerMask pipeLayerMask;
        [SerializeField] private float slideSpeed = 5f;
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float endMargin = 0.1f;
        [SerializeField] private float lateralMovementCap = 0.1f;
        [SerializeField] private float lateralMoveSpeed = 2f;
        [SerializeField] private float immunityTime = 0.5f;
        [SerializeField] private float verticalOffset = 0.5f;

        [SerializeField] private JumpPlayerBehaviour jumpPlayerBehaviour;
        [SerializeField] private AudioSource jumpAudioSource;

        private PipeStageObject currentPipe = null;
        private float currentT1 = 0f;
        private float currentSplineLength = 0f;
        private bool direction = true;
        private float lateralOffset = 0f;
        private float immunityTimer = 0f;

        private Vector3 previousPosition;
        private Vector3 previousUpVector;

        public override bool Evaluate() {
            bool result = false;
            if (immunityTimer > 0f) {
                immunityTimer -= Time.fixedDeltaTime;
                return false;
            }

            if (currentPipe == null) {
                Collider[] colliders = Physics.OverlapSphere(modelTransform.position, detectionRadius, pipeLayerMask, QueryTriggerInteraction.Collide);
                foreach (var col in colliders) {
                    PipeStageObject pipe = col.GetComponent<PipeStageObject>();
                    if (pipe != null) {
                        currentPipe = pipe;
                        SplineUtility.GetNearestPoint(pipe.splineContainer.Spline, pipe.splineContainer.transform.InverseTransformPoint(modelTransform.position), out float3 _, out float closestT);
                        currentT1 = closestT;
                        currentSplineLength = pipe.splineContainer.CalculateLength();
                        lateralOffset = 0.5f;
                        float dist = closestT * currentSplineLength;
                        if (dist <= endMargin || dist >= currentSplineLength - endMargin) {
                            currentPipe = null;
                            currentT1 = 0f;
                            currentSplineLength = 0f;
                            continue;
                        }
                        direction = Vector3.Dot(modelTransform.forward, pipe.splineContainer.Spline.EvaluateTangent(currentT1)) > 0;
                        previousPosition = modelTransform.position;
                        lateralOffset = FindProfilePosOfPlayer(modelTransform.position);
                        result = true;
                        break;
                    }
                }
            }
            else {
                if (inputHandler.jump.pressed) {
                    playerBehaviourTree.groundInformation.ground = false;
                    jumpPlayerBehaviour.Jump(previousUpVector, true);
                    jumpAudioSource.Play();
                    currentPipe = null;
                    immunityTimer = immunityTime;
                    result = false;
                }

                result = true;
            }

            if (result) {
                PositionPlayer();
                return true;
            }

            return false;
        }

        private float FindProfilePosOfPlayer(Vector3 playerPos) {
            //Given a Vector3 position of the player in world space, find the corresponding lateral Value of the player that positions them corrently
            //Find nearest point on spline to player position
            if (currentPipe == null) return 0f;
            SplineUtility.GetNearestPoint(currentPipe.splineContainer.Spline, currentPipe.splineContainer.transform.InverseTransformPoint(playerPos), out float3 nearestPoint, out float closestT);
            Vector3 main_Position = SplineUtility.EvaluatePosition(currentPipe.splineContainer.Spline, currentT1);
            main_Position = currentPipe.splineContainer.transform.TransformPoint(main_Position);
            Vector3 main_Tangent = SplineUtility.EvaluateTangent(currentPipe.splineContainer.Spline, currentT1);
            main_Tangent = currentPipe.splineContainer.transform.TransformDirection(main_Tangent);
            main_Tangent.Normalize();
            Vector3 main_Up = SplineUtility.EvaluateUpVector(currentPipe.splineContainer.Spline, currentT1);
            main_Up = currentPipe.splineContainer.transform.TransformDirection(main_Up);
            main_Up.Normalize();
            Vector3 main_Right = Vector3.Cross(main_Tangent, main_Up).normalized;
            main_Right.Normalize();
            Vector3 VectorToPlayer = playerPos - main_Position;
            float x = Vector3.Dot(VectorToPlayer, main_Right);
            float z = Vector3.Dot(VectorToPlayer, main_Up);
            Vector3 profilePos = new Vector3(x, 0f, z) / currentPipe.ProfileScale;
            //Find nearest point on profile spline to this position
            SplineUtility.GetNearestPoint(currentPipe.profileSplineContainer.Spline, profilePos, out float3 nearestProfilePoint, out float closestProfileT);
            return closestProfileT;
        }

        private void PositionPlayer() {
            if (currentPipe == null) return;

            overrideModelTransform = true;
            kinematic = true;
            moving = true;
            moveVelocity = Vector3.zero;

            // Move along spline
            float deltaT = (slideSpeed / currentSplineLength) * Time.fixedDeltaTime * currentPipe.SpeedMultiplier;
            currentT1 += direction ? deltaT : -deltaT;

            if (currentT1 < 0f || currentT1 > 1f) {
                Dismount();
                return;
            }
            // Apply lateral input (flip input if direction is reversed)
            float inputX = inputHandler.slideMove.value * (direction ? -1f : 1f);
            if (inputHandler.slideMove.hold) {
                lateralOffset += inputX * lateralMoveSpeed * currentPipe.LateralSpeed * Time.fixedDeltaTime;
                if (lateralMovementCap > 0.5f) {
                    lateralMovementCap = 0.49f;
                }
                lateralOffset = Mathf.Clamp(lateralOffset, 0f + lateralMovementCap, 1f - lateralMovementCap);
            }

            Vector3 main_Position = SplineUtility.EvaluatePosition(currentPipe.splineContainer.Spline, currentT1);
            main_Position = currentPipe.splineContainer.transform.TransformPoint(main_Position);

            Vector3 main_Tangent = SplineUtility.EvaluateTangent(currentPipe.splineContainer.Spline, currentT1);
            main_Tangent=currentPipe.splineContainer.transform.TransformDirection(main_Tangent);
            main_Tangent.Normalize();
            Vector3 main_Up = SplineUtility.EvaluateUpVector(currentPipe.splineContainer.Spline, currentT1);
            main_Up = currentPipe.splineContainer.transform.TransformDirection(main_Up);
            main_Up.Normalize();
            Vector3 main_Right = Vector3.Cross(main_Tangent, main_Up).normalized;
            main_Right.Normalize();

            Vector3 profile_Position = SplineUtility.EvaluatePosition(currentPipe.profileSplineContainer.Spline, lateralOffset);
            profile_Position = profile_Position * currentPipe.ProfileScale;
            Vector3 world_Offset = main_Right * profile_Position.x + main_Up * profile_Position.z;
            Vector3 world_Position = main_Position + world_Offset;


            Vector3 profile_Tangent = SplineUtility.EvaluateTangent(currentPipe.profileSplineContainer.Spline, lateralOffset);
            profile_Tangent.Normalize();
            Vector3 main_Profile_Tangent = main_Right * profile_Tangent.x + main_Up * profile_Tangent.z;
            main_Profile_Tangent.Normalize();
            //From this tangent calculate up vector
            Vector3 mainProfile_Up = Vector3.Cross(main_Profile_Tangent, -main_Tangent).normalized;
            //Invert this up vector
            mainProfile_Up = -mainProfile_Up;
            mainProfile_Up.Normalize();
            world_Position += mainProfile_Up * verticalOffset;
            previousUpVector = mainProfile_Up;
            //Invert the maintangent based on direction
            if (!direction)
                main_Tangent = -main_Tangent;
            Quaternion targetRotation = Quaternion.LookRotation(main_Tangent, mainProfile_Up);
            modelTransform.position = world_Position;
            playerTransform.position = world_Position;
            modelTransform.rotation = targetRotation;
            previousPosition = world_Position;
        }

        private void Dismount() {
            overrideModelTransform = false;
            kinematic = false;
            playerBehaviourTree.groundInformation.ground = false;
            currentPipe = null;
            currentT1 = 0f;
            currentSplineLength = 0f;
            lateralOffset = 0.5f;
            immunityTimer = immunityTime;
        }
    }

}
