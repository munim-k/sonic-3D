using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    [SerializeField] private Transform horizontalSlashOrigin;
    [SerializeField] private Transform verticalSlashPrefab;
    [SerializeField] private Transform verticalSlashOrigin;
    [SerializeField] private float slashCooldown = 2f;

    [Header("Spears Attack")]
    [SerializeField] private Transform spearPrefab;
    [SerializeField] private Transform[] spearSpawnPoints;
    private List<Transform> spears;
    [SerializeField] private float spearsDelay = 0.5f;
    private float spearTimer = 0f;
    [SerializeField] private float spearCooldown = 2f;


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
        TransitionToSpears();
        navMeshAgent= GetComponent<NavMeshAgent>();
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
        }
        else {
            spearTimer += Time.deltaTime;
            if (spearTimer >= spearsDelay) {
                spearTimer = 0f;
                print("LaunchedSpear");
                Destroy(spears[0].gameObject);
                spears.RemoveAt(0);
            }
            if (spears.Count == 0) {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
                    TransitionToRings();
                }
                else {
                    TransitionToSlash();
                }
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

    
    private void TransitionToSpears() {
        cooldownTimer = spearCooldown;
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
        cooldownTimer = ringCooldown;
        ringTimer = 0f;
    }

    private void TransitionToSlash() {
        state = State.Slash;
        OnStateChange?.Invoke(state);
        cooldownTimer = slashCooldown;
    }

    public void SpawnHorizontalSlash() {
        if (state == State.Slash) {
            Transform slash = Instantiate(horizontalSlashPrefab, horizontalSlashOrigin.position, horizontalSlashOrigin.rotation);
        }
    }

    public void SpawnVerticalSlash() {
        if (state == State.Slash) {
            Transform slash = Instantiate(verticalSlashPrefab, verticalSlashOrigin.position, verticalSlashOrigin.rotation);
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
        if (state == State.Rings)
            return;
        health -= damage;
        onHit?.Invoke();
        if (health <= 0) {
            state = State.Dead;
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
