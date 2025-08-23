using UnityEngine;

public class PatrollerBotVisual : MonoBehaviour
{
    [SerializeField] private PatrollerBotEnemy patrollerBot;
    [SerializeField] private Animator botAnimator;
    private readonly string MOVEMENT_BOOL = "Moving";

    private void Start() {
        patrollerBot.OnStateChange += HandleStateChange;
    }

    private void HandleStateChange(PatrollerBotEnemy.State newState) {
        //Handle visual changes based on state
        switch (newState) {
            case PatrollerBotEnemy.State.Moving:
                botAnimator.SetBool(MOVEMENT_BOOL, true);
                break;
            case PatrollerBotEnemy.State.Turning:
                botAnimator.SetBool(MOVEMENT_BOOL, false);
                break;
        }
    }
}
