using UnityEngine;

namespace RagdollEngine {
    public class CartPlayerBehaviour : PlayerBehaviour {
        [SerializeField] private LayerMask cartLayerMask;
        [SerializeField] private float cartDetectionRadius = 5f;
        [SerializeField] private float cartJumpDistance = 10f;
        [SerializeField] private float cartJumpTime = 0.1f;
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
        private CartStageObject currentCart = null;
        private CartStageObject nextCart = null;

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
                        CartStageObject cartObject = collider.GetComponent<CartStageObject>();
                        if (cartObject != null) {
                            currentCart = cartObject;
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
                        CartStageObject cartObject = hit.collider.GetComponent<CartStageObject>();
                        if (cartObject != null && cartObject != currentCart) {
                            nextCart = cartObject;
                            isSidestepping = true;
                            sideSteppingTimer = 0f;
                            PositionPlayer();
                            result = true;
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

                Transform cartTransform = currentCart.transform;

                overrideModelTransform = true;
                kinematic = true;
                moving = true;
                moveVelocity = Vector3.zero; // Reset move velocity to prevent unwanted movement during sidestep
                if (isSidestepping) {
                    //If player is sidestepping, lerp between current cart and next cart
                    Transform nextCartTransform = nextCart.transform;
                    float lerpVal = sideSteppingTimer / cartJumpTime;
                    Vector3 targetPosition = Vector3.Lerp(cartTransform.position, nextCartTransform.position, lerpVal);
                    Quaternion targetRotation = Quaternion.Lerp(cartTransform.rotation, nextCartTransform.rotation, lerpVal);
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
                        nextCart = null;
                        isSidestepping = false;
                    }
                }
                else {

                    playerTransform.position = cartTransform.position;
                    modelTransform.position = cartTransform.position;
                    modelTransform.rotation = cartTransform.rotation;
                }

            }
        }


    }
}