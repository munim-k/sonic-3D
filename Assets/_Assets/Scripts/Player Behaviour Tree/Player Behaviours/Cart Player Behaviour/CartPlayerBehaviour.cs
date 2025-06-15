using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RagdollEngine {
    public class CartPlayerBehaviour : PlayerBehaviour {
        [SerializeField] private LayerMask cartLayerMask;
        [SerializeField] private float cartSpeed = 1f;
        [SerializeField] private float cartDetectionRadius = 5f;
        [SerializeField] private float cartJumpDistance = 10f;
        [SerializeField] private float cartJumpTime = 0.1f;
        [SerializeField] private float cartEndMargins = 0.1f;
        [Header("Cart Jump Additive Transform Curves")]
        [SerializeField] private AnimationCurve cartJumpXTransformCurve;
        [SerializeField] private AnimationCurve cartJumpYTransformCurve;
        [SerializeField] private AnimationCurve cartJumpZTransformCurve;
        [Header("Cart Jump Additive Rotation Curves")]
        [SerializeField] private AnimationCurve cartJumpXRotationCurve;
        [SerializeField] private AnimationCurve cartJumpYRotationCurve;
        [SerializeField] private AnimationCurve cartJumpZRotationCurve;
        [SerializeField] private float cartJumpImmunityTime = 1f;
        private float cartJumpImmunityTimer = 0f;
        private CartRailStageObject currentCart = null;
        private CartRailStageObject nextCart = null;

        private float currentCartLerp = 0f;
        private float nextCartLerp = 0f;

        private bool currentCartDirection = true; // True if the cart is moving in the positive direction, false if negative
        private bool nextCartDirection = true; // True if the next cart is moving in the positive direction, false if negative

        private bool isSidestepping = false;
        private float sideSteppingTimer = 0f;
        private bool sideStepDir = false;

        [SerializeField] JumpPlayerBehaviour jumpPlayerBehaviour;

        [SerializeField] AudioSource jumpAudioSource;

        public override bool Evaluate() {
            //If player is intersecting with cart or inside cart
            bool result = false;
            if (cartJumpImmunityTimer > 0f) {
                cartJumpImmunityTimer -= Time.fixedDeltaTime;
                return result;

            }

            if (currentCart == null) {
                // Check if the player is colliding with a cart stage object
                Collider[] colliders = Physics.OverlapSphere(modelTransform.position, cartDetectionRadius, cartLayerMask, QueryTriggerInteraction.Collide);
                // If a cart stage object is found, set cart to true
                if (colliders.Length > 0) {
                    foreach (Collider collider in colliders) {
                        CartRailStageObject cartObject = collider.GetComponent<CartRailStageObject>();
                        if (cartObject != null) {
                            currentCart = cartObject;
                            //Get the closest position on the cart to set the players position to
                            SplineUtility.GetNearestPoint(currentCart.splineContainer.Spline, currentCart.splineContainer.transform.InverseTransformPoint(modelTransform.position), out float3 _, out float closestT);
                            currentCartLerp = closestT;
                            float maxDist = currentCart.splineContainer.CalculateLength();
                            float tDist = closestT * maxDist; // Get the distance along the spline at the closest T value
                            //If closestT is within endMargins or 0 and 1 then dont get into the spline
                            if (tDist >= (maxDist - cartEndMargins) || tDist <= (0 + cartEndMargins)) {
                                currentCart = null;
                                currentCartLerp = 0f;
                                result = false;
                                continue;
                            }
                            //Set cartDirection according to tangent
                            Vector3 splineTangent = currentCart.splineContainer.Spline.EvaluateTangent(currentCartLerp);
                            currentCartDirection = Vector3.Dot(modelTransform.forward, splineTangent) > 0;
                            result = true;
                        }
                    }
                }
            }
            else {
                //If player is currently in cart 
                //Player is sidestepping
                if (isSidestepping)
                    result = true;
                //Player pressed space 
                else if (inputHandler.jump.pressed) {
                    playerBehaviourTree.groundInformation.ground = false;
                    jumpPlayerBehaviour.Jump(Vector3.up, true);
                    jumpAudioSource.Play();
                    currentCart = null;
                    cartJumpImmunityTimer = cartJumpImmunityTime;
                    result = false;
                }
                //Player pressed sidestep
                else if (inputHandler.sidestep.pressed || inputHandler.sidestep.hold) {
                    //If player pressed sideStep then begin transitioning between carts
                    Vector3 jumpDir = modelTransform.right * cartJumpDistance;
                    sideStepDir = true;
                    if (inputHandler.sidestep.value < 0) {
                        jumpDir *= -1; // If sidestep is positive, jump to the left
                        sideStepDir = false;
                    }
                    jumpDir.Normalize();
                    RaycastHit[] hits = Physics.SphereCastAll(modelTransform.position, 2f, jumpDir, cartJumpDistance, cartLayerMask, QueryTriggerInteraction.Collide);
                    foreach (RaycastHit hit in hits) {
                        CartRailStageObject cartObject = hit.collider.GetComponent<CartRailStageObject>();
                        if (cartObject != null && cartObject != currentCart) {
                            nextCart = cartObject;
                            //Get the closest position on the cart to set the players position to
                            SplineUtility.GetNearestPoint(nextCart.splineContainer.Spline, nextCart.splineContainer.transform.InverseTransformPoint(modelTransform.position), out float3 _, out float closestT);
                            nextCartLerp = closestT;
                            //Set cartDirection according to tangent
                            Vector3 splineTangent = nextCart.splineContainer.Spline.EvaluateTangent(nextCartLerp);
                            nextCartDirection = Vector3.Dot(modelTransform.forward, splineTangent) > 0;
                            isSidestepping = true;
                            sideSteppingTimer = 0f;
                            result = true;
                            break;
                        }
                    }
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
            //Control the players modelPosition and modelRotation, set player to kinematic
            if (currentCart != null) {
                overrideModelTransform = true;
                kinematic = true;
                moving = true;
                moveVelocity = Vector3.zero; // Reset move velocity to prevent unwanted movement during sidestep
                if (isSidestepping) {
                    //If player is sidestepping, lerp between current cart and next cart
                    float splineDistance = currentCart.splineContainer.CalculateLength();
                    float currentCartIncrement = (cartSpeed / splineDistance) * Time.fixedDeltaTime;
                    splineDistance = nextCart.splineContainer.CalculateLength();
                    float nextCartIncrement = (cartSpeed / splineDistance) * Time.fixedDeltaTime;

                    if (currentCartDirection) {
                        currentCartLerp += currentCartIncrement;
                        currentCartLerp = math.min(currentCartLerp, 1);
                    }
                    else {
                        currentCartLerp -= currentCartIncrement;
                        currentCartLerp = math.max(currentCartLerp, 0);
                    }

                    if (nextCartDirection) {
                        nextCartLerp += nextCartIncrement;
                        nextCartLerp = math.min(nextCartLerp, 1);
                    }
                    else {
                        nextCartLerp -= nextCartIncrement;
                        nextCartLerp = math.max(nextCartLerp, 0);
                    }

                    //Get the positions and rotation
                    Vector3 currentCartPos;
                    Quaternion currentCartRot;
                    GetSplinePositionAndRotation(currentCart.splineContainer, currentCartLerp, currentCartDirection, out currentCartPos, out currentCartRot);
                    Vector3 nextCartPos;
                    Quaternion nextCartRot;
                    GetSplinePositionAndRotation(nextCart.splineContainer, nextCartLerp, nextCartDirection, out nextCartPos, out nextCartRot);

                    float lerpVal = sideSteppingTimer / cartJumpTime;
                    Vector3 targetPosition = Vector3.Lerp(currentCartPos, nextCartPos, lerpVal);
                    Quaternion targetRotation = Quaternion.Lerp(currentCartRot, nextCartRot, lerpVal);
                    targetPosition.x += cartJumpXTransformCurve.Evaluate(lerpVal);
                    targetPosition.y += cartJumpYTransformCurve.Evaluate(lerpVal);
                    targetPosition.z += cartJumpZTransformCurve.Evaluate(lerpVal);


                    Vector3 targetRotationAddition;
                    targetRotationAddition.x = cartJumpXRotationCurve.Evaluate(lerpVal);
                    targetRotationAddition.y = cartJumpYRotationCurve.Evaluate(lerpVal);
                    targetRotationAddition.z = cartJumpZRotationCurve.Evaluate(lerpVal);
                    if (!sideStepDir) {
                        targetRotationAddition.x *= -1; // If sidestep is positive, flip the x rotation
                    }
                    targetRotation *= Quaternion.Euler(targetRotationAddition);

                    modelTransform.position = targetPosition;
                    modelTransform.rotation = targetRotation;
                    playerTransform.position = targetPosition;
                    sideSteppingTimer += Time.fixedDeltaTime;
                    if (sideSteppingTimer >= cartJumpTime) {
                        //Once lerp is complete, set currentCart to nextCart and reset sidestepping
                        currentCart = nextCart;
                        currentCartDirection = nextCartDirection;
                        currentCartLerp = nextCartLerp;
                        nextCart = null;
                        nextCartLerp = 0;

                        nextCartDirection = false;
                        isSidestepping = false;
                    }
                }
                else {
                    float splineDistance = currentCart.splineContainer.CalculateLength();
                    float speedMultiplier = currentCart.SpeedMultiplier;
                    float increment = (cartSpeed / splineDistance) * Time.fixedDeltaTime * speedMultiplier;
                    if (currentCartDirection) {
                        currentCartLerp += increment;
                        if (currentCartLerp > 1f) {
                            DismountCart();
                            return;
                        }
                    }
                    else {
                        currentCartLerp -= increment;
                        if (currentCartLerp < 0f) {
                            DismountCart();
                            return;
                        }
                    }
                    Vector3 targetPos;
                    Quaternion targetRot;
                    GetSplinePositionAndRotation(currentCart.splineContainer, currentCartLerp, currentCartDirection, out targetPos, out targetRot);
                    playerTransform.position = targetPos;
                    modelTransform.position = targetPos;
                    modelTransform.rotation = targetRot;
                }

            }
        }

        private void GetSplinePositionAndRotation(SplineContainer spline, float lerp, bool dir, out Vector3 position, out Quaternion rotation) {
            position = spline.Spline.EvaluatePosition(lerp);
            position = spline.transform.TransformPoint(position);

            Vector3 splineTangent = spline.Spline.EvaluateTangent(lerp);
            if (!dir)
                splineTangent *= -1;
            Vector3 splineUp = spline.Spline.EvaluateUpVector(lerp);

            rotation = Quaternion.LookRotation(splineTangent, splineUp);
        }

        private void DismountCart() {
            overrideModelTransform = false;
            kinematic = false;
            playerBehaviourTree.groundInformation.ground = false;
            currentCart = null;
            cartJumpImmunityTimer = cartJumpImmunityTime;
        }

    }
}
