using UnityEngine;

public class AnimOnEnemyDeath : MonoBehaviour {
    [SerializeField] private Animation[] animations;
    [SerializeField] private Transform[] enemies;
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

        if (aliveEnemies == 0) {
            foreach (var anim in animations) {
                if (anim != null) {
                    anim.Play();
                }
            }
        }
    }

    private void HandleEnemyDeath() {
        aliveEnemies--;
        if (aliveEnemies == 0) {
            foreach (var anim in animations) {
                if (anim != null) {
                    anim.Play();
                }
            }
        }
    }
}
