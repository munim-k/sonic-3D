using System;
using System.Collections;
using UnityEngine;

public class World8Boss : MonoBehaviour, BaseEnemy {
    [SerializeField] private Transform levelExit;
    //Boss stuff
    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 100;

    private int health = 100;
    private Action onDeath;
    private Action onHit;

    [Header("Staff Attack")]
    [SerializeField] private Transform lightPillarPrefab;
    [SerializeField] private float pillarAttackStep = 0.1f;
    [SerializeField] private float staffAttackDuration = 5f;
    private bool startSpawningPillars = false;
    private float pillarTimer = 0f;

    [Header("Crying Attack")]
    [SerializeField] private Transform explosiveGreenBombPrefab;
    [SerializeField] private Transform bombSpawnPoint;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float bombFlightTime = 1f;
    [SerializeField] private float bombAttackStep = 0.1f;
    [SerializeField] private float cryAttackDuration = 5f;
    private float bombStepTimer = 0f;


    [Header("Tantrum Attack")]
    [SerializeField] private Transform shockwavePrefab;


    private float attackTimer = 0f;
    public enum State {
        Staff,
        Crying,
        Tantrum,
        Dead
    }

    private State state;
    public Action<State> OnStateChange;
    Action BaseEnemy.OnDeath { get => onDeath; set => onDeath = value; }
    Action IHittable.OnHit { get => onHit; set => onHit = value; }



    void Start() {
        health = maxHealth;
        levelExit.gameObject.SetActive(false);
        state = State.Staff;
        attackTimer = staffAttackDuration;
        pillarTimer = pillarAttackStep;
        StartCoroutine(InitialEvent());
    }

    IEnumerator InitialEvent() {
        yield return null;
        OnStateChange?.Invoke(state);
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.Staff:
                Staff();
                break;
            case State.Crying:
                Crying();
                break;
            case State.Tantrum:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    private void Staff() {
        if (!startSpawningPillars)
            return;
        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f) {
            if (RandomBool()) {
                state = State.Tantrum;
            }
            else {
                state = State.Crying;
                attackTimer = cryAttackDuration;
            }
            startSpawningPillars = false;
            OnStateChange?.Invoke(state);
            return;
        }
        pillarTimer -= Time.fixedDeltaTime;
        if (pillarTimer <= 0f) {
            pillarTimer = pillarAttackStep;
            Vector3 spawnPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
            spawnPos.y = 0;
            if (lightPillarPrefab != null)
                Instantiate(lightPillarPrefab, spawnPos, Quaternion.identity);
        }
    }

    // Called by animation event
    public void StartSpawningPillars() {
        if (state != State.Staff)
            return;
        startSpawningPillars = true;
    }

    private void Crying() {
        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f) {
            if (RandomBool()) {
                state = State.Tantrum;
            }
            else {
                state = State.Staff;
                attackTimer = staffAttackDuration;
            }
            OnStateChange?.Invoke(state);
            return;
        }
        RotateTowardsPlayer();
        bombStepTimer -= Time.fixedDeltaTime;
        if (bombStepTimer <= 0f) {
            bombStepTimer = bombAttackStep;
            if (explosiveGreenBombPrefab != null && bombSpawnPoint != null) {
                Transform bomb = Instantiate(explosiveGreenBombPrefab, bombSpawnPoint.position, Quaternion.identity);
                Vector3 direction = GetLaunchVelocity(bombFlightTime, bombSpawnPoint.position, Player.CharacterInstance.playerBehaviourTree.modelTransform.position);
                bomb.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
            }
        }
    }


    private Vector3 GetLaunchVelocity(float flightTime, Vector3 start, Vector3 target) {
        Vector3 gravity = Physics.gravity; // e.g. (0, –9.81f, 0)

        // 1. Horizontal displacement (projected onto plane perpendicular to gravity)
        Vector3 gravityDir = gravity.normalized;
        Vector3 startFlat = start - Vector3.Project(start, gravityDir);
        Vector3 targetFlat = target - Vector3.Project(target, gravityDir);
        Vector3 horizontalDisplacement = targetFlat - startFlat;

        Vector3 horizontalVelocity = horizontalDisplacement / flightTime;

        // 2. Vertical displacement (along gravity direction)
        Vector3 verticalDisplacement = Vector3.Project(target - start, gravityDir);
        Vector3 gravityEffect = 0.5f * gravity * (flightTime * flightTime);
        Vector3 verticalVelocity = (verticalDisplacement - gravityEffect) / flightTime;

        return horizontalVelocity + verticalVelocity;
    }

    public void EndTantrum() {
        //Tantrum handled by animation event
        if (RandomBool()) {
            state = State.Staff;
            attackTimer = staffAttackDuration;
        }
        else {
            state = State.Crying;
            attackTimer = cryAttackDuration;
        }
        OnStateChange?.Invoke(state);
    }

    public void SpawnTrantrumShockwave(Vector3? p = null) {
        Vector3 pos = p ?? transform.position;
        if (shockwavePrefab != null) {
            Instantiate(shockwavePrefab, pos, Quaternion.identity);
        }
    }

    private bool RandomBool() {
        return UnityEngine.Random.value > 0.5f;
    }

    private void RotateTowardsPlayer() {
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        Vector3 direction = (playerPos - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);
    }


    void IHittable.DoHit(int damage) {
        if (state == State.Staff)
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
