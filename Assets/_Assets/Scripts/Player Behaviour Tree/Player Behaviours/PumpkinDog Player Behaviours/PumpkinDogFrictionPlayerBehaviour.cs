using UnityEngine;

namespace RagdollEngine {
    public class PumpkinDogFrictionPlayerBehaviour : PlayerBehaviour {

        [SerializeField] float minFriction;

        [SerializeField] float maxFriction;


        [SerializeField] float maxSpeed;


        public override void Execute() {
            if (!moving)
                Slow();
            active = true;
        }


        void Slow() {
            additiveVelocity -= moveVelocity * Mathf.Lerp(minFriction, maxFriction, 1 - Mathf.Pow(10, -(moveVelocity.magnitude / maxSpeed)));
        }


    }
}
