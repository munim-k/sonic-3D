using UnityEngine;

namespace RagdollEngine {

    public class CartCheckPlayerBehaviour : PlayerBehaviour {
        [SerializeField] private CartPlayerBehaviour cart;

        public override bool Evaluate() {
            return cart.active;
        }
    }

}