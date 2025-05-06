using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    //Enemy is hit by attacks, and takes damage, actual behaviour of enemy is defined in child classes
    [SerializeField] protected int maxHealth = 100;
    protected int health = 0;
    public Action onDeath;
    public Action onDamage;
    
    private  void Start()
    {
        health = maxHealth;
    }

    public virtual void DoDamageToEnemy(int damage)
    {
        health -= damage;
        onDamage?.Invoke();
        if (health <= 0)
        {
            onDeath?.Invoke();
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual int GetHealth()
    {
        return health;
    }


}
