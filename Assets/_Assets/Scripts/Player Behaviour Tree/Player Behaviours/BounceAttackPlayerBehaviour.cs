using UnityEngine;

namespace RagdollEngine {
    public class BounceAttackPlayerBehaviour : PlayerBehaviour {
        //Makes the player bounce off of the top of enemies and deal damage to them
        [SerializeField] private float dounwardsRayDistance = 0.25f;
        [SerializeField] private int bounceDamage = 20;
        [SerializeField] private int bounceForce = 5;
        [SerializeField] private LayerMask enemyLayerMask;
        public override bool Evaluate() {
            //Check if player is colliding with an enemy, if so then apply damage to enemy and upward bounce force
            RaycastHit[] hits;
            hits = Physics.RaycastAll(playerTransform.position, Vector3.down, dounwardsRayDistance, enemyLayerMask);
            if (hits.Length == 0) {
                return false; // No enemies hit
            }
            foreach (RaycastHit hit in hits) {
                if (hit.collider.gameObject != null) {
                    BaseEnemy enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();
                    if (enemy != null) {
                        //Apply damage to enemy
                        enemy.DoHit(bounceDamage);
                        //Apply upward bounce force
                        Vector3 velocity = additiveVelocity;
                        velocity.y = -RB.linearVelocity.y;
                        Vector3 acceleration = accelerationVector;
                        acceleration.y = 0;
                        velocity += Vector3.up * bounceForce;
                        additiveVelocity = velocity;
                        accelerationVector = acceleration;
                        return true; // Bounce attack successful

                    }
                }
            }
            return false;
        }


    }

}
