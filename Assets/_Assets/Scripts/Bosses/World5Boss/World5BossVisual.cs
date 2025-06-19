using UnityEngine;

public class World5BossVisual : MonoBehaviour {

    [SerializeField] private Animator animator;
    [SerializeField] private World5Boss boss;
    private readonly string MOVING_BOOL = "Walking";
    private readonly string SMASH_PARAM = "Smash";
    private readonly string DEATH_PARAM = "Dead";
    void Start() {
        boss.OnStateChange += UpdateAnimationState;
    }

    // Update is called once per frame
    private void UpdateAnimationState(World5Boss.State s) {
        switch (s) {
            case World5Boss.State.Moving:
                animator.SetBool(MOVING_BOOL, true);
                break;
            case World5Boss.State.SmashAttack:
                animator.SetTrigger(SMASH_PARAM);
                break;
            case World5Boss.State.ProjectileAttack:
                animator.SetBool(MOVING_BOOL, false);
                break;
            case World5Boss.State.Death:
                animator.SetBool(DEATH_PARAM, true);
                break;
            default:
                break;
        }
    }
}
