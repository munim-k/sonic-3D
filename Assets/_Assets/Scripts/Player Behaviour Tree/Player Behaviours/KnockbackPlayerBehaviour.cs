using UnityEngine;

namespace RagdollEngine {

    public class KnockbackPlayerBehaviour : PlayerBehaviour {

        private Vector3 knockbackDir;
        [SerializeField] private float drag = 0.1f;
        [SerializeField] private float knockbackGroundImmunityDuration = 0.1f;
        private float knockbackGroundTimer = 0f;
        public override void Execute() {
            knockbackDir = Vector3.Lerp(knockbackDir, Vector3.zero, drag * Time.fixedDeltaTime);

            knockbackGroundTimer -= Time.fixedDeltaTime;
            if (knockbackGroundTimer < 0f && groundInformation.ground) {
                knockbackDir = Vector3.zero;
            }

            additiveVelocity += knockbackDir;

        }
        public void ApplyKnockback(Vector3 dir) {
            additiveVelocity += dir;
            knockbackDir += dir;
            knockbackDir.y = 0;
            knockbackGroundTimer = knockbackGroundImmunityDuration;
        }

    }
}
