using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeStepperBotEnemy : MonoBehaviour, BaseEnemy, IHittable
{

    public enum State {
        Idle,
        Moving,
        AttackWindup,
        Attacking,
        Retreating

    }
    [SerializeField] private int maxHealth = 100;
    private int currentHealth = 0;
    [SerializeField] private GameObject deathExplosion;

    private Action On_Death;
    private Action On_Hit;
    private State state = State.Idle;
    public Action<State> OnStateChange;


    private NavMeshAgent navMeshAgent;

    //Idle State
    [Header("Idle State Settings")]
    [SerializeField] private float playerDetectionRadius = 5f;

    //Moving State
    [Header("Moving State Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    //AttackWindup State
    [Header("AttackWindup State Settings")]
    [SerializeField] private float attackWindupTime = 1f;
    [SerializeField] private float attackRange = 1.5f;
    private float attackWindupTimer = 0f;

    //Attacking State
    //Dependent entirely on animation states
    [Header("Attacking State Settings")]
    private bool startAttack= false;
    [SerializeField] private DamageVolume attackHitbox;
    [SerializeField] private float attackTravelTime = 0.5f;
    private float attackTravelTimer = 0f;
    private Vector3 oldAttackPosition;
    private Vector3 oldplayerPos;
    
    Action BaseEnemy.OnDeath { get => On_Death; set => On_Death = value; }
    Action IHittable.OnHit { get => On_Hit; set => On_Hit = value; }


    private void Start() {
        currentHealth = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        OnStateChange?.Invoke(state);
        if (attackHitbox != null) {
            attackHitbox.gameObject.SetActive(false);
        }
    }
    private void FixedUpdate() {
        switch (state) {
            case State.Idle:
                Idle();
                break;
            case State.Moving:
                Moving();
                break;
            case State.AttackWindup:
                AttackWindup();
                break;
            case State.Attacking:
                if (startAttack) {
                Attacking();
                }
                break;
            case State.Retreating:
                Retreating();
                break;
            default:
                break;
        }
    }

    private void Idle() {
        // Implement Idle behavior
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        if (Vector3.SqrMagnitude(transform.position-playerPos) <= playerDetectionRadius*playerDetectionRadius) {
            state = State.Moving;
            OnStateChange?.Invoke(state);
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.SetDestination(playerPos);
        }
    }

    private void Moving() {
        // Implement Moving behavior
        if (navMeshAgent != null) {
            navMeshAgent.speed = moveSpeed;
            Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
            navMeshAgent.SetDestination(playerPos);
            if (Vector3.SqrMagnitude(transform.position - playerPos) <= attackRange * attackRange) {
                state = State.AttackWindup;
                OnStateChange?.Invoke(state);
                attackWindupTimer = attackWindupTime;

                navMeshAgent.speed = 0f;
                navMeshAgent.isStopped = true;
              
            }
        }
    }

    private void AttackWindup() {
        // Implement AttackWindup behavior
        attackWindupTimer -= Time.fixedDeltaTime;
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        Vector3 vectorToPlayer = (playerPos - transform.position).normalized;
        vectorToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        if (attackWindupTimer <= 0) {
            attackWindupTimer = 0;
            state = State.Attacking;
            OnStateChange?.Invoke(state);
            oldAttackPosition = transform.position;
            oldplayerPos = playerPos;
            attackTravelTimer = attackTravelTime;
            attackHitbox.damageDealt = false;
            attackHitbox.gameObject.SetActive(true);
            return;
        }
        if (Vector3.SqrMagnitude(transform.position - playerPos) > attackRange * attackRange) {
            state = State.Moving;
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = moveSpeed;
            OnStateChange?.Invoke(state);
            return;
        }

    }
    public void StartAttack() {
        //Called by animation event
        startAttack = true;
    }
    private void Attacking() {
        attackTravelTimer -= Time.fixedDeltaTime;
        float t = 1f - (attackTravelTimer / attackTravelTime);
        transform.position = Vector3.Lerp(oldAttackPosition, oldplayerPos, t);
        if (attackTravelTimer < 0) {
            attackTravelTimer = attackTravelTime;
            state = State.Retreating;
            OnStateChange?.Invoke(state);
            startAttack = false;
            return;
        }
    }

    private void Retreating() {
        // Implement Retreating behavior
        attackTravelTimer -= Time.fixedDeltaTime;
        float t = 1f- attackTravelTimer / attackTravelTime;
        transform.position = Vector3.Lerp(oldplayerPos, oldAttackPosition, t);
        if (attackTravelTimer < 0) {
            attackTravelTimer = 0;
            state = State.Idle;
            navMeshAgent.Warp(oldAttackPosition);
            navMeshAgent.isStopped = true;
            attackHitbox.gameObject.SetActive(false);
            OnStateChange?.Invoke(state);
            return;
        }
    }



    void IHittable.DoHit(int damage) {
        currentHealth -= damage;
        On_Hit?.Invoke();
        if (currentHealth <= 0) {
            On_Death?.Invoke();
            if (deathExplosion != null) {
                Instantiate(deathExplosion, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }

    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)currentHealth / (float)maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }

}
