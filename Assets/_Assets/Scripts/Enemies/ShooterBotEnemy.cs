using System;
using UnityEngine;

public class ShooterBotEnemy : MonoBehaviour, BaseEnemy, IHittable {

    public enum State {
        Idle,
        Attack,
        Dead
    }
    private State state;
    [SerializeField] private int maxHealth = 20;

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    public Action OnAttack;
    public Action<State> OnStateChange;
    private Action OnHit;
    private Action OnDeath;
    Action BaseEnemy.OnDeath {
        get => OnDeath;
        set => OnDeath = value;
    }

    Action IHittable.OnHit {
        get => OnHit;
        set => OnHit = value;
    }

    private int health;
    private float attackTimer;

    private void Start() {
        state = State.Idle;
        health = maxHealth;
    }
    private void Update() {
        switch (state) {
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

    private void Idle() {
        //If player is within attackRange then go to attack mode
        if (Vector3.SqrMagnitude(transform.position - Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position) <= attackRange * attackRange) {
            state = State.Attack;
            OnStateChange?.Invoke(state);
        }

    }

    private void Attack() {
        //Rotate the enemy to face the player


        if (attackTimer <= 0) {
            //Attack
            OnAttack?.Invoke();
            //Set players current position as the target to fire at
            Vector3 targetPosition = Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position;
            targetPosition.y += 1f;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectile.GetComponent<ShooterBotProjectile>().SetLaunchDir(targetPosition - firePoint.position);
            attackTimer = attackCooldown;
        }
        else {
            attackTimer -= Time.deltaTime;
            //If player is out of attackRange then go to idle mode
            if (Vector3.SqrMagnitude(transform.position - Player.CharacterInstance.playerBehaviourTree.modelTransform.transform.position) > attackRange * attackRange) {
                state = State.Idle;
                OnStateChange?.Invoke(state);
            }
        }


    }


    public float GetHealthNormalized() {
        return (float)health / maxHealth;
    }
    private void Dead() {
        //Do nothing, enemy is dead

    }

    public void DoHit(int damage) {
        health -= damage;
        OnHit?.Invoke();
        if (health <= 0) {
            OnDeath?.Invoke();
            //Turn off the collider on this gameObject
            Collider col = this.GetComponent<Collider>();
            if (col != null) {
                col.enabled = false;
            }
            state = State.Dead;
            OnStateChange?.Invoke(state);
        }
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}
