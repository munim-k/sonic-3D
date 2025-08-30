using UnityEngine;

public class World8BossVisual : MonoBehaviour {


    [SerializeField] private World8Boss boss;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform deathExplosionVisuals;
    [SerializeField] private Transform shieldVisual;
    private readonly string STAFF_START_TRIGGER = "StaffStart";
    private readonly string CRY_START_TRIGGER = "CryStart";
    private readonly string TANTRUM_START_TRIGGER = "TantrumStart";

    void Start() {
        boss.OnStateChange += OnStateChange;
    }

    private void OnStateChange(World8Boss.State s) {
        shieldVisual.gameObject.SetActive(false);
        switch (s) {
            case World8Boss.State.Staff:
                animator.SetTrigger(STAFF_START_TRIGGER);
                shieldVisual.gameObject.SetActive(true);
                break;
            case World8Boss.State.Crying:
                animator.SetTrigger(CRY_START_TRIGGER);
                break;
            case World8Boss.State.Tantrum:
                animator.SetTrigger(TANTRUM_START_TRIGGER);
                break;
            case World8Boss.State.Dead:
                animator.speed = 0f;
                deathExplosionVisuals.gameObject.SetActive(true);
                deathExplosionVisuals.parent = null;
                break;
            default:
                break;
        }
    }
}
