using UnityEngine;

public class World6BossVisual : MonoBehaviour
{
    [SerializeField] private World6Boss boss;
    [SerializeField] private Animator animator;
    private readonly string SPINNING_PARAMETER = "Spin";
    private readonly string JUMPING_PARAMETER = "Jump";
    private readonly string MOVING_BOOLEAN = "Moving";
    private readonly string DEAD_PARAMETER = "Dead";
    private void Start() {
        boss.OnStateChange+= OnStateChange;
    }

    private void OnStateChange(World6Boss.State s) {
        animator.SetBool(MOVING_BOOLEAN, false);
        switch (s) {
            case World6Boss.State.Spinning:
                animator.SetTrigger(SPINNING_PARAMETER);
                break;
            case World6Boss.State.Jumping:
                animator.SetTrigger(JUMPING_PARAMETER);
                break;
            case World6Boss.State.Moving:
                    animator.SetBool(MOVING_BOOLEAN, true);
                break;
            case World6Boss.State.Dead:
                animator.SetTrigger(DEAD_PARAMETER);
                break;
            default:
                break;
        }
    }
}
