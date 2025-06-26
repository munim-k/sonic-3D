using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Image health;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private GameObject enableObject;
    
    [SerializeField] private GameObject enemyPrefab;

    private void Start()
    {
        currentHealth = maxHealth;
        if (enableObject != null)
        {
            enableObject.SetActive(false); // Ensure the object is disabled at the start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            currentHealth -= 10;
            health.fillAmount = (float)currentHealth / maxHealth;
            if (currentHealth <= 0)
            {
                if (enableObject != null)
                {
                    enableObject.SetActive(true); // Enable the object when health is zero
                }
                Destroy(enemyPrefab); // Destroy the enemy game object
            }
        }
    }
}
