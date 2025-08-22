using RagdollEngine;
using System;
using UnityEngine;

public class ShieldGunnerEnemy : MonoBehaviour, BaseEnemy, IHittable {
    public enum State {
        Idle,
        Shooting
    }
    //Base Variables
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private GameObject deathExplosion;


    //Idle state
    [SerializeField] private float detectionRange = 15f;

    //Shooting state
    [SerializeField] private Transform gunTransform;
    [SerializeField] private LayerMask raycastLayersToAvoid;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float bulletStep = 0.5f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float fastRotationAngle = 90f;
    [SerializeField] private float rotationSlowFactor = 0.5f;
    [SerializeField] private float gunRotationSpeed = 2f;
    //Enemy will only fire if player is inside this angle
    [SerializeField] private float maxFiringAngle = 0.5f;

    private DamagePlayerBehaviour playerDamageBehaviour;
    private float bulletTimer = 0f;
    //Variables shared across Functions
    private Vector3 playerPosition;
    private bool firing = false;
    //
    private State state;

    private Action On_Death;
    private Action On_Hit;

    public Action<State> OnStateChange;
    public Action<bool> FiringStateChange;
    Action BaseEnemy.OnDeath { get => On_Death; set => On_Death = value; }
    Action IHittable.OnHit { get => On_Hit; set => On_Hit = value; }

    private void Awake() {
        state = State.Idle;
        currentHealth = maxHealth;
    }



    private void FixedUpdate() {
        playerPosition = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        playerPosition.y += 1f;
        switch (state) {
            case State.Idle:
                Idle_FixedUpdate();
                break;
            case State.Shooting:
                Shooting_FixedUpdate();
                break;
            default:
                break;
        }
    }

    private void Idle_FixedUpdate() {
        if (Vector3.SqrMagnitude(playerPosition - transform.position) < detectionRange * detectionRange) {
            state = State.Shooting;
            OnStateChange?.Invoke(state);
        }
    }



    private void Shooting_FixedUpdate() {
        if (Vector3.SqrMagnitude(playerPosition - transform.position) > detectionRange * detectionRange) {
            state = State.Idle;
            OnStateChange?.Invoke(state);
            return;
        }
        Vector3 playerDir = playerPosition - transform.position;
        float angle = Vector3.Angle(transform.forward, playerDir);
        //Rotate towards player
        Vector3 rotationDir = playerDir;
        rotationDir.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(rotationDir);
        //If player is behind the enemy, slow rotation speed by slow factor
        float finalRotationSpeed = rotationSpeed;
        if (angle > fastRotationAngle) {
            finalRotationSpeed *= rotationSlowFactor;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, finalRotationSpeed * Time.fixedDeltaTime);

        if (angle < maxFiringAngle) {
            Vector3 playerDirFromGun = playerPosition - gunTransform.position;
            Quaternion gunRotation = Quaternion.LookRotation(playerDirFromGun);
            gunTransform.rotation = Quaternion.Lerp(gunTransform.rotation, gunRotation, gunRotationSpeed * Time.fixedDeltaTime);
            if (!firing) {
                firing = true;
                FiringStateChange?.Invoke(firing);
            }
        }
        else if (firing) {
            firing = false;
            FiringStateChange?.Invoke(firing);
        }
        if (bulletTimer > 0f) {
            bulletTimer -= Time.fixedDeltaTime;
            if (bulletTimer <= 0f) {
                bulletTimer = bulletStep;
            }
            else {
                return; //Wait for next bullet step
            }
        }
        if (angle < maxFiringAngle) {
            Vector3 shotOrigin = gunTransform.position + gunTransform.forward * 2;
            Debug.DrawLine(shotOrigin, gunTransform.position + gunTransform.forward * 100f, Color.red, bulletStep);
            if (Physics.Raycast(shotOrigin, gunTransform.forward, out RaycastHit hit, Mathf.Infinity, ~raycastLayersToAvoid, QueryTriggerInteraction.Ignore)) {
                //Bullet hits something
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0) {
                    //Hit object is on player layer
                    if (playerDamageBehaviour == null) {
                        foreach (PlayerBehaviour b in Player.CharacterInstance.playerBehaviourTree.behaviours) {
                            if (b is DamagePlayerBehaviour) {
                                playerDamageBehaviour = b as DamagePlayerBehaviour;
                            }
                        };
                    }
                    playerDamageBehaviour.DoDamage(bulletDamage);
                }
            }
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
            Destroy(gameObject);
        }
    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)currentHealth / (float)maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }


}
