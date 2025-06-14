using UnityEngine;

namespace RagdollEngine {
    public class LollipopSoundEffect : SoundEffect {
        [SerializeField] LollipopCollectionPlayerBehaviour lollipopPlayerBehaviour;
        private float additivePitch = 0f;
        private float pitchStep = 0.04f;
        private float pitchTime = 1f;
        private float pitchTimer = 0f;
        private float maxAdditivePitch = 0.4f;
        public override bool Evaluate() {
            active = lollipopPlayerBehaviour.collectedLollipop;
            if (active) {
                additivePitch = Mathf.Clamp(additivePitch + pitchStep, 0f, maxAdditivePitch);
                pitchTimer = pitchTime;
            }

            if (pitchTimer > 0f) {
                pitchTimer -= Time.deltaTime;
            }
            else {
                additivePitch = 0;
            }
            effects[0].audioSource.pitch = 1f + additivePitch;

            return active;
        }
    }
}