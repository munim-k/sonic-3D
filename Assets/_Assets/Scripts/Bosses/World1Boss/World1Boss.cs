using System;
using System.Collections.Generic;
using UnityEngine;

public class World1Boss : MonoBehaviour, BaseEnemy, IHittable {
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int maxHealth = 100;
    private int currentHealth = 100;

    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackStep = 0.1f;
    [SerializeField] private float attackDuration = 1f;

    [SerializeField] private Transform attackPointCentre;
    [SerializeField] private AnimationCurve attackRotationCurve;
    [SerializeField] private float attackRotationSpeed = 1f;
    [SerializeField] private Transform[] attackPoints;
    [SerializeField] private Transform attackProjectile;
    private float attackRotationLerp = 0f;
    private Quaternion attackRotationOriginal;

    [SerializeField] private float stunDuration = 2f;
    private float stunTimer = 0f;
    [SerializeField] private World1BossPillar[] pillars;
    [SerializeField] private GameObject pillarCentre;
    [SerializeField] private AnimationCurve pillarRotationCurve;
    [SerializeField] private float pillarRotationSpeed = 1f;
    private float pillarRotationLerp = 0f;
    private Quaternion pillarRotationOriginal;
    private List<World1BossPillar> activePillars = new List<World1BossPillar>();

    [SerializeField] private GameObject levelExit;


    private float attackTimer = 0f;
    private float attackCooldownTimer = 0f;
    private float attackStepTimer = 0f;


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
    private State state;
    public enum State {
        Spinning,
        Stunned,
        Dead
    }

    void Awake() {
        currentHealth = maxHealth;
        state = State.Spinning;
        attackRotationOriginal = attackPointCentre.localRotation;
        pillarRotationOriginal = pillarCentre.transform.localRotation;
        attackTimer = attackDuration;
        attackStepTimer = attackStep;
        InitializePillars();
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            //If cooldown timer is 0 start attack timer and do attack  until attack timer is 0 then reset cooldown timer

            case State.Spinning:
                Spinning();
                break;
            case State.Stunned:
                Stunned();
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    private void Spinning() {
        if (attackCooldownTimer > 0f) {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f) {
                attackTimer = attackDuration;
                attackStepTimer = attackStep;
            }
        }

        if (attackTimer > 0f) {
            attackTimer -= Time.fixedDeltaTime;
            Attack();
            if (attackTimer <= 0f) {
                //Attack
                attackCooldownTimer = attackCooldown;
            }
        }
        //Spin the pillars
        pillarRotationLerp += Time.fixedDeltaTime * pillarRotationSpeed;
        if (pillarRotationLerp > 1f) {
            pillarRotationLerp = 0f;
        }
        Vector3 eulerAngles = pillarRotationOriginal.eulerAngles;
        float l = pillarRotationCurve.Evaluate(pillarRotationLerp);
        pillarCentre.transform.localRotation = Quaternion.Euler(0f, eulerAngles.y + (l * 360f), 0f);

        //Spin the attack points
        attackRotationLerp += Time.fixedDeltaTime * attackRotationSpeed;
        if (attackRotationLerp > 1f) {
            attackRotationLerp = 0f;
        }
        eulerAngles = attackRotationOriginal.eulerAngles;
        l = attackRotationCurve.Evaluate(attackRotationLerp);
        attackPointCentre.transform.localRotation = Quaternion.Euler(0f, eulerAngles.y + (l * 360f), 0f);
    }

    private void Attack() {
        if (attackStepTimer > 0f) {
            attackStepTimer -= Time.fixedDeltaTime;
        }
        else {
            attackStepTimer = attackStep;
            //Instantiate projectile
            foreach (Transform attackPoint in attackPoints) {
                Transform projectile = Instantiate(attackProjectile, attackPoint.position, attackPoint.rotation);
                projectile.GetComponent<ShooterBotProjectile>().SetLaunchDir(attackPoint.forward);
            }
        }
    }


    public void InitializePillars() {
        List<int> randomIndexes = new List<int>();
        foreach (World1BossPillar p in pillars) {
            p.Activate(false);
        }
        for (int i = 0; i < 3; i++) {
            int randomIndex = UnityEngine.Random.Range(0, pillars.Length);
            while (randomIndexes.Contains(randomIndex)) {
                randomIndex = UnityEngine.Random.Range(0, pillars.Length);
            }
            randomIndexes.Add(randomIndex);
        }

        for (int i = 0; i < randomIndexes.Count; i++) {
            World1BossPillar pillar = pillars[randomIndexes[i]];
            pillar.Activate(true);
            activePillars.Add(pillar);
        }
    }

    public void ActivatePillar(World1BossPillar pillar) {
        if (activePillars.Contains(pillar)) {
            foreach (World1BossPillar p in activePillars) {
                p.Activate(false);
            }
            activePillars.Clear();
            state = State.Stunned;
            stunTimer = stunDuration;
            OnStateChange?.Invoke(state);

        }

    }



    private void Stunned() {
        if (stunTimer > 0f) {
            stunTimer -= Time.fixedDeltaTime;
        }
        else {
            state = State.Spinning;
            InitializePillars();
            OnStateChange?.Invoke(state);
        }
    }



    public float GetHealthNormalized() {
        return (float)currentHealth / maxHealth;
    }

    public void DoHit(int damage) {
        if (state != State.Stunned) {
            return;
        }
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0) {
            currentHealth = 0;
            state = State.Dead;
            levelExit.SetActive(true);
            OnDeath?.Invoke();
            OnStateChange?.Invoke(state);
        }
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}
