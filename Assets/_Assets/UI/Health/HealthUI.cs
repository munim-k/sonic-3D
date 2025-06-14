using RagdollEngine;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    [SerializeField] private Image healthBar;
    private DamagePlayerBehaviour playerBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void Initialize(DamagePlayerBehaviour playerBehaviour) {
        playerBehaviour.onDamage += SetHealth;

    }


    public void SetHealth(int health, int maxHealth) {
        healthBar.fillAmount = (float)health / maxHealth;
    }
}
