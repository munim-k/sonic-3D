using System;
using UnityEngine;

public class World4Boss : MonoBehaviour, BaseEnemy
{

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
    [SerializeField] private Spikes[] spikes;
    [SerializeField] private bool spikesActive = true;
    [SerializeField] private float spikeDamage = 10f;
    [SerializeField] private float spikeDuration = 3f;
    [SerializeField] private float spikeCooldown = 5f;
    void ToggleSpikes()
    {
        spikesActive = !spikesActive;
        foreach (Spikes spike in spikes)
        {
            spike.SetSpikesState(spikesActive);
        }
    }

    private float attackTimer = 0f;
    private float attackCooldownTimer = 0f;

    public Action OnDamage;
    public Action<State> OnStateChange;

    private State state;
    public enum State
    {
        ProjectileAttacks,
        Spikes,
        Dead
    }

    void Awake()
    {
        currentHealth = maxHealth;
        state = State.Spikes;
        OnStateChange?.Invoke(state);
        attackTimer = spikeDuration;
        attackCooldownTimer = 0.1f;
        movement = GetComponent<World4BossMovement>();
        movement.SetMovement(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Spikes:
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

    private void ProjectileAttacks()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f)
            {
                attackTimer = projectileAttackDuration;
                projectileAttackTimer = projectileAttackStep;
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (projectileAttackTimer > 0f)
            {
                projectileAttackTimer -= Time.fixedDeltaTime;
            }
            if (projectileAttackTimer <= 0f)
            {
                //Instantiate one projectile attack on each transform
                foreach (Transform projectilettackTransform in projectileAttackTransforms)
                {
                    Instantiate(projectileAttackPrefab, projectilettackTransform.position, projectilettackTransform.rotation);
                }
                projectileAttackTimer = projectileAttackStep;
            }

            if (attackTimer <= 0f)
            {
                attackCooldownTimer = projectileAttackCooldown;
                state = State.Spikes;
                OnStateChange?.Invoke(state);
            }
        }


    }

    private void Spikes()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f)
            {
                ToggleSpikes();
                attackTimer = spikeDuration;
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                ToggleSpikes();
                attackCooldownTimer = spikeCooldown;
                state = State.ProjectileAttacks;
                OnStateChange?.Invoke(state);
            }
        }
    }

    public void DoDamageToEnemy(int damage)
    {
        if (state != State.ProjectileAttacks)
        {
            return;
        }
        currentHealth -= damage;
        OnDamage?.Invoke();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            state = State.Dead;
            levelExit.SetActive(true);
            OnStateChange?.Invoke(state);
        }
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth / maxHealth;
    }
}
