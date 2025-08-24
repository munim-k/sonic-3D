using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SquasherBotEnemy : MonoBehaviour, BaseEnemy, IHittable {
    public enum State {
        Idle,
        Squashing,
        Squashed,
        Rising
    }

    [SerializeField] private int maxHealth = 100;
    private int currentHealth = 0;
    [SerializeField] private GameObject deathExplosion;

    private Action On_Death;
    private Action On_Hit;
    private State state = State.Idle;
    public Action<State> OnStateChange;

    [SerializeField] private DamageVolume damageVolume;
    //Idle State
    [SerializeField] private float sphereCastRadius = 1.1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask layersToExclude;
    //Squashing State
    [SerializeField] private float descendTime = 0.1f;
    [SerializeField] private float descendOffset = 0.5f;
    private Vector3 originalPos;
    private Vector3 squashPos;
    private float descendProgress = 0f;

    //Squashed State
    [SerializeField] private float squashedTime = 1f;
    private float squashedTimer = 0f;

    //Rising State
    [SerializeField] private float riseTime = 0.1f;
    private float riseTimer = 0f;

    private Vector3 raycastHitPos;

    private Rigidbody rb;
    private void Start() {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    Action BaseEnemy.OnDeath { get => On_Death; set => On_Death = value; }
    Action IHittable.OnHit { get => On_Hit; set => On_Hit = value; }

    private void FixedUpdate() {
        switch (state) {
            case State.Idle:
                Idle();
                break;
            case State.Squashing:
                Squashing();
                break;
            case State.Squashed:
                Squashed();
                break;
            case State.Rising:
                Rising();
                break;
            default:
                break;
        }
    }
    private void Idle() {
        //Do a spherecast to detect player
        Vector3 flattenedPos = rb.position;
        flattenedPos.y = 0f;
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        playerPos.y = 0f;
        if (Vector3.SqrMagnitude(flattenedPos - playerPos) >= sphereCastRadius * sphereCastRadius) {
            return;
        }
        //Only do spherecast if player is within horizontal distance of sphereCastRadius
        if (Physics.SphereCast(rb.position, sphereCastRadius, transform.rotation * Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, playerLayer, QueryTriggerInteraction.Ignore)) {
            //If player detected then do a linecast to see if there is anything in the way
            if (!Physics.Linecast(rb.position, hitInfo.point, ~layersToExclude, QueryTriggerInteraction.Ignore)) {
                //If nothing in the way then do another spherecast to find the ground
                float descendDistance = hitInfo.distance + descendOffset;
                if (Physics.SphereCast(rb.position, sphereCastRadius, transform.rotation * Vector3.down, out hitInfo, Mathf.Infinity, ~layersToExclude & ~playerLayer, QueryTriggerInteraction.Ignore)) {
                    //If ground found then set descend distance to ground distance
                    descendDistance = Vector3.Distance(rb.position,hitInfo.point) + descendOffset;
                }
                //Start squashing
                state = State.Squashing;
                    damageVolume.damageDealt = false;
                    OnStateChange?.Invoke(state);
                    descendProgress = 0f;
                    originalPos = rb.position;
                    squashPos = originalPos + (transform.rotation * (Vector3.down * descendDistance));
            }
        }
    }

    private void Squashing() {
        descendProgress += Time.fixedDeltaTime;
        float t = descendProgress / descendTime;
        rb.MovePosition(Vector3.Lerp(originalPos, squashPos, t));
        if (t >= 1f) {
            state = State.Squashed;
            OnStateChange?.Invoke(state);
            squashedTimer = 0f;
        }
    }

    private void Squashed() {
        squashedTimer += Time.fixedDeltaTime;
        if (squashedTimer >= squashedTime) {
            state = State.Rising;
            OnStateChange?.Invoke(state);
            riseTimer = 0f;
        }
    }

    private void Rising() {
        riseTimer += Time.fixedDeltaTime;
        float t = riseTimer / riseTime;
        rb.MovePosition(Vector3.Lerp(squashPos, originalPos, t));
        if (t >= 1f) {
            state = State.Idle;
            squashPos = Vector3.zero;
            OnStateChange?.Invoke(state);
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
            Destroy(gameObject.transform.parent.gameObject);
        }

    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)currentHealth / (float)maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }


}
