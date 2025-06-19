using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Image health;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "BombProjectile(Clone)")
        {
            currentHealth -= 10;
            health.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
