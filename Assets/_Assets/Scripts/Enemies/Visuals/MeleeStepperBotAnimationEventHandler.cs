using UnityEngine;

public class MeleeStepperBotAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private MeleeStepperBotEnemy enemy;

    public void StartAttack() {
        enemy.StartAttack();
    }
}
