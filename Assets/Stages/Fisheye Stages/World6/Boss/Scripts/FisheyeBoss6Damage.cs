using System;
using UnityEngine;
using UnityEngine.UI;

public class FisheyeBoss6Damage : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameObject boss;
    [SerializeField] private Image health;
    private float currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
        Bottle.OnBottleHitBoss += TakeDamage;
    }

    private void TakeDamage()
    {
        currentHealth -= 10f; // Example damage value
        health.fillAmount = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            Bottle.OnBottleHitBoss -= TakeDamage;
            Destroy(boss);
        }
    }

    private void OnDestroy()
    {
        Bottle.OnBottleHitBoss -= TakeDamage; // Unsubscribe to avoid memory leaks
    }
}
