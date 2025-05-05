using System;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    //Enemy is hit by attacks, and takes damage, actual behaviour of enemy is defined in child classes
    [SerializeField] private int health = 100;

    public Action onDeath;
    public Action onDamage;
    

    public virtual void DoDamage(int damage)
    {
        health -= damage;
        onDamage?.Invoke();
        if (health < 0)
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
