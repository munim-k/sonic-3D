using UnityEngine;

public class World7BossVisual : MonoBehaviour
{
    [SerializeField] private World7Boss boss;
    [SerializeField] private Animator animator;
    private readonly string MOVEMENT_BOOL = "Moving";
    private readonly string SLASH_TRIGGER = "Slash";
    private readonly string RING_ENTER_TRIGGER = "RingEnter";
    private readonly string RING_EXIT_TRIGGER = "RingExit";
    private readonly string DEATH_BOOL = "Dead";

     void Start()
    {
        boss.OnStateChange += OnStateChange;
    }

    private void OnStateChange(World7Boss.State s) {
        animator.SetBool(MOVEMENT_BOOL, false);
        switch (s) {
            case World7Boss.State.Spears:
                animator.SetBool(MOVEMENT_BOOL, true);
                break;
            case World7Boss.State.Rings:
                animator.SetTrigger(RING_ENTER_TRIGGER);
                break;
            case World7Boss.State.RingsExit:
                animator.SetTrigger(RING_EXIT_TRIGGER);
                break;
            case World7Boss.State.Slash:
                animator.SetTrigger(SLASH_TRIGGER);
                break;
            case World7Boss.State.Dead:
                animator.SetBool(DEATH_BOOL,true);
                break;
            default:
                break;
        }
    }

    
}
