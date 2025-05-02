using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class PumpkinDogAimPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] private float maxSpeed; // Maximum speed the player can move
        bool aiming;
        public override bool Evaluate()
        {
            aiming = inputHandler.roll.hold;
            return aiming;
        }


        


    }
}
