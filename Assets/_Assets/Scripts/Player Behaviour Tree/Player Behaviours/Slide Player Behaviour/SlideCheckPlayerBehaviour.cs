using UnityEngine;

namespace RagdollEngine {

    public class SlideCheckPlayerBehaviour : PlayerBehaviour {
        [SerializeField] private SlidePlayerBehaviour slide;

        // This method checks if the player is currently sliding.
        public override bool Evaluate() {
            return slide.active;
        }
    }
}