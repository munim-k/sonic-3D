using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class World7Boss : MonoBehaviour, BaseEnemy {
    [SerializeField] private Transform levelExit;
    //Boss stuff
    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 100;
    private int health = 100;
    private Action onDeath;
    private Action onHit;

    [Header("Rings Attack")]
    [SerializeField] private Transform ringPrefab;
    [SerializeField] private float ringDelay;
    [SerializeField] private int ringNum = 0;
    [SerializeField] private float ringCooldown = 2f;
    private float ringTimer = 0f;
    private bool startSpawningRings = false;

    [Header("Slash Attack")]
    [SerializeField] private Transform horizontalSlashPrefab;
    [SerializeField] private Transform verticalSlashPrefab;
    [SerializeField] private Transform horizontalSlashOrigin;
    [SerializeField] private Transform verticalSlashOrigin;
    [SerializeField] private float slashCooldown = 2f;

    [Header("Spears Attack")]
    [SerializeField] private Transform spearPrefab;
    [SerializeField] private Transform[] spearSpawnPoints;
    private List<Transform> spears;
    [SerializeField] private float spearsDelay = 0.5f;
    private float spearTimer = 0f;
    private int currentAttack = 0;

    private float cooldownTimer = 0f;

    public enum State {
        Spears,
        Rings,
        RingsExit,
        Slash,
        Dead
    }

    private State state;
    public Action<State> OnStateChange;
    private NavMeshAgent navMeshAgent;
    Action BaseEnemy.OnDeath { get => onDeath; set => onDeath = value; }
    Action IHittable.OnHit { get => onHit; set => onHit = value; }



    void Start() {
        spears = new List<Transform>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        TransitionToSpears();
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case State.Spears:
                SpearsState();
                break;
            case State.Rings:
                RingsState();
                break;
            case State.Slash:
                SlashState();
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    private void SpearsState() {
        if (cooldownTimer >= 0f) {
            cooldownTimer -= Time.deltaTime;
            navMeshAgent.destination=Player.CharacterInstance.playerBehaviourTree.modelTransform.position; // Move towards player
            if (cooldownTimer < 0) {
                navMeshAgent.Warp(transform.position);
                navMeshAgent.updatePosition = false;
                navMeshAgent.updateRotation = false;
                navMeshAgent.destination = transform.position;
            }
        }
        else {
            RotateTowardsPlayer();
            spearTimer += Time.deltaTime;
            if (spearTimer >= spearsDelay) {
                spearTimer = 0f;
                spears[0].parent = null;
                spears[0].GetComponent<World7BossSpear>().Fire();
                spears.RemoveAt(0);
            }
            if (spears.Count == 0) {
                if (currentAttack==0) {
                    TransitionToRings();
                }
                else {
                    TransitionToSlash();
                }
                currentAttack += 1;
                currentAttack%= 2; // Reset to 0 after 2 attacks
            }
        }
    }

    private void RingsState() {
        if (ringNum >= 3)
            return;
        ringTimer += Time.deltaTime;
        if (ringTimer >= ringDelay) {
            ringTimer = 0f;
            if (ringNum == 0) {
                print("Ring1");
            }
            else if (ringNum == 1) {
                print("Ring2");
            }
            else {
                state= State.RingsExit;
                OnStateChange?.Invoke(state);
            }
            ringNum++;
        }

    }

    private void SlashState() {
        RotateTowardsPlayer();
     }

    private void RotateTowardsPlayer() {
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        Vector3 direction = (playerPos - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    private void TransitionToSpears() {
        switch (state) {
            case State.Rings:
                cooldownTimer = ringCooldown;
                break;
            case State.RingsExit:
                cooldownTimer = ringCooldown;
                break;
            case State.Slash:
                cooldownTimer = slashCooldown;
                break;
            default:
                break;
        }
        navMeshAgent.ResetPath();
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
        navMeshAgent.Warp(transform.position);
        
        spearTimer = 0f;
        spears.Clear();
        for (int i = 0; i < spearSpawnPoints.Length; i++) {
            Transform spear = Instantiate(spearPrefab, spearSpawnPoints[i]);
            spears.Add(spear);
        }
        state = State.Spears;
        OnStateChange?.Invoke(state);
    }


    private void TransitionToRings() {
      
        state = State.Rings;
        OnStateChange?.Invoke(state);
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true; // Stop the NavMeshAgent from moving
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = false;
        navMeshAgent.Warp(transform.position);
        
        ringTimer = 0f;
    }

    private void TransitionToSlash() {
        state = State.Slash;
        OnStateChange?.Invoke(state);
        navMeshAgent.isStopped = true; // Stop the NavMeshAgent from moving
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = false;
        navMeshAgent.Warp(transform.position);
    }

    public void SpawnHorizontalSlash() {
        if (state == State.Slash) {
            Transform slash = Instantiate(horizontalSlashPrefab, horizontalSlashOrigin.position, horizontalSlashOrigin.rotation);
        }
    }

    public void SpawnVerticalSlash() {
        if (state == State.Slash) {
            Transform slash = Instantiate(verticalSlashPrefab, verticalSlashOrigin.position,verticalSlashOrigin.rotation);
        }
    }

    public void StartSpawningRings() {
        startSpawningRings = true;
        ringNum = 0;
        ringTimer = ringDelay;
    }
    public void ExitRingState() {
        startSpawningRings = false;
        ringNum = 0;
        ringTimer = 0f;
        TransitionToSpears();
    }
    public void EndSlash() {
        if (state == State.Slash) {
            TransitionToSpears();
        }
    }
    void IHittable.DoHit(int damage) {
        if (state == State.Rings || state==State.RingsExit)
            return;
        health -= damage;
        onHit?.Invoke();
        if (health <= 0) {
            state = State.Dead;
            levelExit.gameObject.SetActive(true);
            onDeath?.Invoke();
            OnStateChange?.Invoke(state);
        }
    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)health / maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}
