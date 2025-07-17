using UnityEngine;

public class FisheyeBoss6Animation : MonoBehaviour
{
    private const string ATTACK_TRIGGER = "isAttacking";
    [SerializeField] private Animator animator;
    [SerializeField] private FisheyeBoss6Attacks bossAttacks;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (bossAttacks != null)
        {
            bossAttacks.OnAttack += TriggerAttackAnimation;
        }
    }

    private void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }
    }
}
