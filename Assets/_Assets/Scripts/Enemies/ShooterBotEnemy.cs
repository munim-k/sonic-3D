using System;
using UnityEngine;

public class ShooterBotEnemy : BaseEnemy
{

    public enum State{
        Idle,
        Attack,
        Dead
    }
    private State state;

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    public Action OnAttack;
    public Action OnHit;
    public Action OnDeath;

    public Action<State> OnStateChange;

    private float attackTimer;

    private void Start()
    {
        state = State.Idle;
        health = maxHealth;
    }
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Dead:
                Dead();
                break;
            default:
                break;
        }

    }

    private void Idle()
    {
        //If player is within attackRange then go to attack mode
        if (Vector3.SqrMagnitude(transform.position- Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position) <= attackRange*attackRange)
        {
            state = State.Attack;
            OnStateChange?.Invoke(state);
        }

    }

    private void Attack()
    {
        //Rotate the enemy to face the player
      

        if (attackTimer <= 0)
        {
            //Attack
            OnAttack?.Invoke();
            //Set players current position as the target to fire at
            Vector3 targetPosition = Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position;
            targetPosition.y += 1f;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectile.GetComponent<ShooterBotProjectile>().SetLaunchDir(targetPosition-firePoint.position);
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
             //If player is out of attackRange then go to idle mode
            if (Vector3.SqrMagnitude(transform.position - Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position) > attackRange * attackRange)
        {
            state = State.Idle;
            OnStateChange?.Invoke(state);
        }
        }


    }

    public override void DoDamageToEnemy(int damage)
    {
        base.health -= damage;
        onDamage?.Invoke();
        if (base.health <= 0)
        {
            onDeath?.Invoke();
            state = State.Dead;
            OnStateChange?.Invoke(state);
        }
    }

    private void Dead()
    {
        //Do nothing, enemy is dead

    }
}
