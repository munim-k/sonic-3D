using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class PumpkinDogAimPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] private float throwForce; // Force applied to the projectile

        bool aiming;
        public override bool Evaluate()
        {
            aiming = inputHandler.roll.hold;
            active = aiming;

            if(wasActive && !active)
            {
                //Reset the move velocity to the models forward direction
                moveVelocity = Vector3.ProjectOnPlane(modelTransform.forward, plane) ;
            }
            return aiming;
        }

        public override void Execute()
        {
           //Do raycasts for projectile motion based on the camera angle

        }


        


    }
}
