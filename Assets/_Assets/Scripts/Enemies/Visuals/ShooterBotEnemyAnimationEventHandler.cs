using UnityEngine;

public class ShooterBotEnemyAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private ShooterBotEnemy shooterBot;
    public void DieEnd() {
        shooterBot.EndDeath();
    }
}
