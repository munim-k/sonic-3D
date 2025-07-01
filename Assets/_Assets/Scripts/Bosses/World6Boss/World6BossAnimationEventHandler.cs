using UnityEngine;

public class World6BossAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private World6Boss world6Boss;

    private void EnableThrowing() {
        world6Boss.SetBombThrowing(true);
    }

    private void DisableThrowing() {
        world6Boss.SetBombThrowing(false);
    }

    private void JumpLand() {
        world6Boss.SpawnJumpShockwave();
    }

    private void SpinStop() {
        world6Boss.TransitionToMoving(World6Boss.State.Spinning);
    }

    private void JumpStop() {
        world6Boss.TransitionToMoving(World6Boss.State.Jumping);

    }
}
