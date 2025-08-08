using System;
using UnityEngine;

public class BeeEnemy : MonoBehaviour, BaseEnemy, IHittable {

    public enum State {
        Idle,   //Bee is idly hovering
        Looking, //Bee is rotating to look at player
        Charging //Bee is charging towards player
    }

    private State state;
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;
    [SerializeField] private Rigidbody RB;
    //Looking state
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float lookDuration = 2f;
    [SerializeField] private float lookSpeed = 2f;
    private float lookTimer = 0f;

    //Charging State
    [SerializeField] private float chargingSpeed = 5f;
    [SerializeField] private float detonationRange = 2f;
    [SerializeField] private float detonationDuration = 0.5f;
    private float detonationTimer = 0f;
    private Vector3 chargeDir = Vector3.zero;
    private Vector3 chargePoint = Vector3.zero;


    //Explosion
    [SerializeField] private GameObject damageSphere;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private GameObject explosionVFX;

    private Action On_Death;
    private Action On_Hit;

    public Action OnDeath { get => On_Death; set => On_Death = value; }
    public Action OnHit { get => On_Hit; set => On_Hit = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        state = State.Idle;
    }

    void FixedUpdate() {
        switch (state) {
            case State.Idle:
                Idle();
                break;
            case State.Looking:
                Looking();
                break;
            case State.Charging:
                Charging();
                break;
            default:
                break;
        }

    }

    private void Idle() {
        Vector3 playerPosition = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        if (Vector3.SqrMagnitude(playerPosition - transform.position) < detectionRange * detectionRange) {
            state = State.Looking;
            lookTimer = lookDuration;
        }
        // Optionally, Can add idle behavior here, like hovering or random movement
    }

    private void Looking() {
        lookTimer -= Time.fixedDeltaTime;
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        if (lookTimer <= 0f) {
            state = State.Charging;
        }
        else {
            RB.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerPos - transform.position), Time.fixedDeltaTime * lookSpeed));
        }
    }

    private void Charging() {
        // Check if the bee is close enough to detonate
        chargePoint = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        if (Vector3.SqrMagnitude(chargePoint - transform.position) < detonationRange * detonationRange) {
            detonationTimer -= Time.fixedDeltaTime;
            if (detonationTimer <= 0f) {
                Explode();
            }
        }
        else {
            chargeDir = chargePoint - transform.position;
            RB.MovePosition(RB.position + chargingSpeed * Time.fixedDeltaTime * chargeDir.normalized);
            RB.MoveRotation(Quaternion.LookRotation(chargeDir));
            detonationTimer = detonationDuration;
        }
    }

    private void Explode() {
        if (damageSphere != null) {
            damageSphere.SetActive(true);
            damageSphere.transform.parent = null;
            Destroy(damageSphere, 2f); // Destroy the damage sphere after 2 seconds
        }
        if (explosionVFX != null) {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)currentHealth / maxHealth;
    }

    void IHittable.DoHit(int damage) {
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0) {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}
