using System;
using UnityEngine;
using UnityEngine.AI;

public class World5Boss : MonoBehaviour, BaseEnemy {
    [SerializeField] private Transform levelExit;
    [SerializeField] private float maxHealth = 200;
    private float currentHealth;

    //Movement
    [SerializeField] private NavMeshAgent navMeshAgent;
    //Smash attack 
    [SerializeField] private Transform smashAttackHitbox;
    private DamageVolume smashAttackDamageVolume;
    [SerializeField] private float smashAttackDetectionRadius = 2f;
    [SerializeField] private float smashAttackDuration = 2;
    [SerializeField] private float smashAttackCooldown = 3;

    //Projectile Attack
    [SerializeField] private Transform[] projectileSpawnPoints;
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private float projectileAttackCooldown = 2;
    [SerializeField] private float projectileAttackDuration = 5;
    [SerializeField] private float projectileAttackStep = 0.5f;
    private float projectileAttackTimer = 0f;


    private float cooldownTimer = 0f;
    private float durationTimer = 0f;

    public Action<State> OnStateChange;
    private Action OnDeath;
    private Action OnHit;
    Action BaseEnemy.OnDeath { get => OnDeath; set => OnDeath = value; }
    Action IHittable.OnHit { get => OnHit; set => OnHit = value; }

    public enum State {
        Moving,
        SmashAttack,
        ProjectileAttack,
        Death
    }
    private State state;
    void Start() {
        currentHealth = maxHealth;
        state = State.ProjectileAttack;
        durationTimer = projectileAttackDuration;
        smashAttackDamageVolume = smashAttackHitbox.GetComponent<DamageVolume>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.Moving:
                Moving();
                break;
            case State.SmashAttack:
                SmashAttack();
                break;
            case State.ProjectileAttack:
                ProjectileAttack();
                break;
            case State.Death:
                break;
            default:
                break;
        }
    }

    private void Moving() {
        if (cooldownTimer > 0f) {
            cooldownTimer -= Time.fixedDeltaTime;
        }
        //Find the players location and set the navMeshAgent destination
        Vector3 targetPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        navMeshAgent.SetDestination(targetPos);
        //If within certain distance of player trigger smash attack
        if (cooldownTimer <= 0f) {
            if (Vector3.Distance(transform.position, targetPos) < smashAttackDetectionRadius) {
                state = State.SmashAttack;
                navMeshAgent.updatePosition = false;// Stop the NavMeshAgent from moving
                durationTimer = smashAttackDuration;
                smashAttackDamageVolume.damageDealt = false; // Reset the damage dealt flag
                OnStateChange?.Invoke(state);
            }
        }
    }

    private void SmashAttack() {

        durationTimer -= Time.fixedDeltaTime;
        if (durationTimer <= 0f) {
            cooldownTimer = smashAttackCooldown;
            state = State.ProjectileAttack;
            smashAttackHitbox.gameObject.SetActive(false);
            OnStateChange?.Invoke(state);
        }

    }

    private void ProjectileAttack() {
        if (cooldownTimer > 0f) {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer <= 0f) {
                durationTimer = projectileAttackDuration;
                projectileAttackTimer = projectileAttackStep;
            }
            else {
                return;
            }
        }
        if (durationTimer > 0f) {
            durationTimer -= Time.fixedDeltaTime;
            projectileAttackTimer -= Time.fixedDeltaTime;
            if (projectileAttackTimer <= 0f) {
                foreach (var spawnPoint in projectileSpawnPoints) {
                    Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                }
                projectileAttackTimer = projectileAttackStep;
            }
            if (durationTimer <= 0f) {
                cooldownTimer = projectileAttackCooldown;
                state = State.Moving;
                navMeshAgent.Warp(transform.position); // Reset the NavMeshAgent position to the current position
                navMeshAgent.updatePosition = true; // Allow the NavMeshAgent to move again
                navMeshAgent.updateRotation = true; // Allow the NavMeshAgent to rotate again
                OnStateChange?.Invoke(state);
            }

        }
    }
    void IHittable.DoHit(int damage) {
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0) {
            OnDeath?.Invoke();
            navMeshAgent.SetDestination(transform.position);
            state = State.Death;
            levelExit.gameObject.SetActive(true);
            OnStateChange?.Invoke(state);
        }
    }

    float BaseEnemy.GetHealthNormalized() {
        return currentHealth / maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }

}
