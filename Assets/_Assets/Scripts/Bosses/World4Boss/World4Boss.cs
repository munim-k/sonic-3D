using System;
using UnityEngine;

public class World4Boss : MonoBehaviour, BaseEnemy {

    // Movement
    private World4BossMovement movement;

    // Heatlh
    [SerializeField] private int maxHealth = 200;
    private int currentHealth = 200;

    // Projectile Attacks
    [SerializeField] private Transform projectileAttackPrefab;
    [SerializeField] private Transform[] projectileAttackTransforms;
    [SerializeField] private float projectileAttackCooldown = 2f;
    [SerializeField] private float projectileAttackStep = 0.1f;
    [SerializeField] private float projectileAttackDuration = 5f;
    private float projectileAttackTimer = 0f;
    [SerializeField] private GameObject levelExit;


    // Spikes
    [SerializeField] private Transform damageVolume;
    [SerializeField] private float spikeAttackCooldown = 1f;
    [SerializeField] private float spikeAttackDuration = 3f;

    private float attackCooldownTimer = 0f;
    private float attackDurationTimer = 0f;

    public Action<State> OnStateChange;
    private Action OnHit;
    private Action OnDeath;

    private State state;

    Action BaseEnemy.OnDeath { get => OnDeath; set => OnDeath = value; }
    Action IHittable.OnHit { get => OnHit; set => OnHit = value; }

    public enum State {
        ProjectileAttacks,
        SpikesAttack,
        Dead
    }

    void Awake() {
        currentHealth = maxHealth;
        state = State.SpikesAttack;
        OnStateChange?.Invoke(state);
        attackDurationTimer = spikeAttackDuration;
        damageVolume.gameObject.SetActive(true);
        movement = GetComponent<World4BossMovement>();
        movement.SetAttackDuration(spikeAttackDuration);
        movement.StartMoving();
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.SpikesAttack:
                Spikes();
                break;
            case State.ProjectileAttacks:
                ProjectileAttacks();
                break;
            case State.Dead:
                break;
            default:
                break;
        }

    }

    private void ProjectileAttacks() {
        //First spend time in cooldown
        if (attackCooldownTimer > 0f) {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f) {
                projectileAttackTimer = projectileAttackStep;
                attackDurationTimer = projectileAttackDuration;
            }
            else {
                return;
            }
        }
        //After cooldown is over attack for however long the attackDurationTimer goes
        if (attackDurationTimer > 0f) {
            attackDurationTimer -= Time.fixedDeltaTime;
            if (projectileAttackTimer > 0f) {
                projectileAttackTimer -= Time.fixedDeltaTime;
            }
            if (projectileAttackTimer <= 0f) {
                //Instantiate one projectile attack on each transform
                foreach (Transform projectilettackTransform in projectileAttackTransforms) {
                    Instantiate(projectileAttackPrefab, projectilettackTransform.position, projectilettackTransform.rotation);
                }
                projectileAttackTimer = projectileAttackStep;
            }

            if (attackDurationTimer <= 0f) {
                attackCooldownTimer = projectileAttackCooldown;
                state = State.SpikesAttack;
                damageVolume.gameObject.SetActive(true);
                movement.StartMoving();
                OnStateChange?.Invoke(state);
            }
        }


    }

    private void Spikes() {
        if (attackCooldownTimer > 0f) {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f) {
                attackDurationTimer = spikeAttackDuration;
            }
            else {
                return;
            }
        }
        //After cooldown is over attack for however long the attackDurationTimer goes
        if (attackDurationTimer > 0f) {
            attackDurationTimer -= Time.fixedDeltaTime;
            if (attackDurationTimer <= 0f) {
                attackCooldownTimer = spikeAttackCooldown;
                state = State.ProjectileAttacks;
                damageVolume.gameObject.SetActive(false);
                OnStateChange?.Invoke(state);
            }
        }
    }


    public float GetHealthNormalized() {
        return (float)currentHealth / maxHealth;
    }


    public void DoHit(int damage) {
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0) {
            currentHealth = 0;
            state = State.Dead;
            movement.StopMoving();
            damageVolume.gameObject.SetActive(false);
            levelExit.SetActive(true);
            OnDeath?.Invoke();
            OnStateChange?.Invoke(state);
        }
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}
