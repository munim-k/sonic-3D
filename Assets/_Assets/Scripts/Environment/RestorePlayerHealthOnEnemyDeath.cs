using RagdollEngine;
using UnityEngine;

public class RestorePlayerHealthOnEnemyDeath : MonoBehaviour {
    [SerializeField] private int healthToRestore = 20;
    [SerializeField] private bool additive = true; // If true, adds to current health. If false, sets health to healthToRestore.
    [SerializeField] private Transform[] enemies;
    private DamagePlayerBehaviour playerDamageBehaviour;
    int aliveEnemies;
    private void Start() {
        foreach (var enemy in enemies) {
            BaseEnemy enemyComponent = enemy.GetComponent<BaseEnemy>();
            if (enemyComponent != null) {
                enemyComponent.OnDeath += HandleEnemyDeath;
            }
            else {
                print($"AnimOnEnemyDeath: Enemy {enemy.name} does not have a BaseEnemy component.");
                enemies = System.Array.FindAll(enemies, e => e != enemy);
            }
        }
        aliveEnemies = enemies.Length;

    }

    private void HandleEnemyDeath() {
        aliveEnemies--;
        if (aliveEnemies == 0) {
            RestorePlayerHealth();
        }
    }

    private void RestorePlayerHealth() {
        if (playerDamageBehaviour == null) {
            foreach (PlayerBehaviour b in Player.CharacterInstance.playerBehaviourTree.behaviours) {
                if (b is DamagePlayerBehaviour) {
                    playerDamageBehaviour = b as DamagePlayerBehaviour;
                }
            }
                       ;
        }
        int
            currentHealth = healthToRestore;
        if (additive) {
            currentHealth += playerDamageBehaviour.GetCurrentHealth();
        }
        playerDamageBehaviour.SetHealth(currentHealth);

    }
}
