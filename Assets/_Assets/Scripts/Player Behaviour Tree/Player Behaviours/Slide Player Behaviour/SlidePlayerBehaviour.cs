using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine {
    public class SlidePlayerBehaviour : PlayerBehaviour {
        [SerializeField] private LayerMask slideLayerMask;
        [SerializeField] private float slideSpeed = 5f;
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float endMargin = 0.1f;
        [SerializeField] private float lateralMoveSpeed = 5f;
        [SerializeField] private float lateralLimit = 2f; // How far left/right from center the player can go
        [SerializeField] private float immunityTime = 0.5f;
        [SerializeField] private float verticalOffset = 0.5f;

        [SerializeField] private JumpPlayerBehaviour jumpPlayerBehaviour;
        [SerializeField] private AudioSource jumpAudioSource;

        private SlideStageObject currentSlide = null;
        private float currentT = 0f;
        private bool direction = true;
        private float lateralOffset = 0f;
        private float immunityTimer = 0f;

        private Vector3 previousPosition;

        public override bool Evaluate() {
            bool result = false;
            if (immunityTimer > 0f) {
                immunityTimer -= Time.fixedDeltaTime;
                return false;
            }

            if (currentSlide == null) {
                Collider[] colliders = Physics.OverlapSphere(modelTransform.position, detectionRadius, slideLayerMask, QueryTriggerInteraction.Collide);
                foreach (var col in colliders) {
                    SlideStageObject slide = col.GetComponent<SlideStageObject>();
                    if (slide != null) {
                        currentSlide = slide;
                        SplineUtility.GetNearestPoint(slide.splineContainer.Spline, slide.splineContainer.transform.InverseTransformPoint(modelTransform.position), out float3 _, out float closestT);
                        currentT = closestT;
                        float len = slide.splineContainer.CalculateLength();
                        float dist = closestT * len;
                        if (dist <= endMargin || dist >= len - endMargin) {
                            currentSlide = null;
                            currentT = 0f;
                            continue;
                        }
                        direction = Vector3.Dot(modelTransform.forward, slide.splineContainer.Spline.EvaluateTangent(currentT)) > 0;
                        previousPosition = modelTransform.position;
                        result = true;
                        break;
                    }
                }
            }
            else {
                if (inputHandler.jump.pressed) {
                    playerBehaviourTree.groundInformation.ground = true;
                    jumpPlayerBehaviour.Jump(Vector3.up, true);
                    jumpAudioSource.Play();
                    currentSlide = null;
                    immunityTimer = immunityTime;
                }
                result = true;
            }

            if (result) {
                PositionPlayer();
                return true;
            }

            return false;
        }

        private void PositionPlayer() {
            if (currentSlide == null) return;

            overrideModelTransform = true;
            kinematic = true;
            moving = true;
            moveVelocity = Vector3.zero;

            // Move along spline
            float splineLength = currentSlide.splineContainer.CalculateLength();
            float deltaT = (slideSpeed / splineLength) * Time.fixedDeltaTime * currentSlide.SpeedMultiplier;
            currentT += direction ? deltaT : -deltaT;

            if (currentT < 0f || currentT > 1f) {
                Dismount();
                return;
            }

            Vector3 splinePos = currentSlide.splineContainer.Spline.EvaluatePosition(currentT);
            Vector3 tangent = currentSlide.splineContainer.Spline.EvaluateTangent(currentT);
            Vector3 up = currentSlide.splineContainer.Spline.EvaluateUpVector(currentT);
            Vector3 right = Vector3.Cross(up, tangent).normalized;

            splinePos = currentSlide.splineContainer.transform.TransformPoint(splinePos);
            tangent = currentSlide.splineContainer.transform.TransformDirection(tangent);
            up = currentSlide.splineContainer.transform.TransformDirection(up);
            right = currentSlide.splineContainer.transform.TransformDirection(right);

            // Apply lateral input (flip input if direction is reversed)
            float inputX = inputHandler.slideMove.value * (direction ? 1f : -1f);
            if (inputHandler.slideMove.hold) {
                lateralOffset += inputX * lateralMoveSpeed * Time.fixedDeltaTime;
                lateralOffset = Mathf.Clamp(lateralOffset, -lateralLimit, lateralLimit);
            }

            Vector3 offsetPos = splinePos + right * lateralOffset + Vector3.up * verticalOffset;
            Vector3 movementDir = (offsetPos - previousPosition).normalized;
            Quaternion rot = movementDir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(movementDir, up) : modelTransform.rotation;

            playerTransform.position = offsetPos;
            modelTransform.position = offsetPos;
            modelTransform.rotation = rot;

            previousPosition = offsetPos;
        }

        private void Dismount() {
            overrideModelTransform = false;
            kinematic = false;
            playerBehaviourTree.groundInformation.ground = false;
            currentSlide = null;
            currentT = 0f;
            lateralOffset = 0f;
            immunityTimer = immunityTime;
        }
    }
}
