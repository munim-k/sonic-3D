using UnityEngine;

public class MeleeStepperBotVisual : MonoBehaviour
{
    [SerializeField] private MeleeStepperBotEnemy enemy;
    [SerializeField] private Animator meleeBotAnimator;
    private readonly string MOVING_BOOL = "Moving";
    private readonly string WINDUP_TRIGGER = "Windup";
    private readonly string ATTACK_TRIGGER = "Attack";

    private void Start() {
        enemy.OnStateChange+= HandleStateChange;
    }
    private void HandleStateChange(MeleeStepperBotEnemy.State newState) {
        switch (newState) {
            case MeleeStepperBotEnemy.State.Idle:
                meleeBotAnimator.SetBool(MOVING_BOOL, false);
                break;
            case MeleeStepperBotEnemy.State.Moving:
                meleeBotAnimator.SetBool(MOVING_BOOL, true);
                break;
            case MeleeStepperBotEnemy.State.AttackWindup:
                meleeBotAnimator.SetBool(MOVING_BOOL, false);
                meleeBotAnimator.SetTrigger(WINDUP_TRIGGER);
                break;
            case MeleeStepperBotEnemy.State.Attacking:
                meleeBotAnimator.SetBool(MOVING_BOOL, false);
                meleeBotAnimator.SetTrigger(ATTACK_TRIGGER);
                break;
            case MeleeStepperBotEnemy.State.Retreating:
                meleeBotAnimator.SetBool(MOVING_BOOL, false);
                break;
            default:
                break;
        }
    }
}
