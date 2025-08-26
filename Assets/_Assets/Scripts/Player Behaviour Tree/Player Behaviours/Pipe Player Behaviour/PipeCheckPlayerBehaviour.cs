using UnityEngine;

namespace RagdollEngine {

    public class PipeCheckPlayerBehaviour : PlayerBehaviour {
        [SerializeField] private PipePlayerBehaviour pipe;

        public override bool Evaluate() {
            return pipe.active;
        }
    }

}
