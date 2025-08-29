using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
[RequireComponent(typeof(Rigidbody))]
public class HelicopterBotEnemy : MonoBehaviour, BaseEnemy, IHittable {

    private enum State {
        FollowPath,
        FollowPlayer,
        ReturnToPath,

    }

    //Shared variables
    [SerializeField] private float maxHealth = 40f;
    private float currentHealth;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float flightMinHeight = 5f;
    [SerializeField] private GameObject deathExplosion;
    private Rigidbody rb;

    //FollowPath
    [SerializeField] private SplineContainer followSplineContainer;
    private Spline followSpline = null;
    [SerializeField] private float playerDetectionRadius = 10f;


    //FollowPlayer
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileFireStep = 0.2f;
    [SerializeField] private float missileFireDuration = 1f;
    [SerializeField] private float missileFireCooldown = 1f;
    [SerializeField] private float playerDetectionRadiusFollowPlayer = 15f;
    [SerializeField] private float movementLerpSpeed = 2f;
    private float missileFireStepTimer = 0f;
    private float missileFireTimer = 0f;
    private float missileFireCooldownTimer = 0f;
    private Vector3 playerPosition;

    //ReturnToPath
    [SerializeField] private float frontObstacleDetectionDistance = 5f;
    [SerializeField] private float frontObstacleSphereCastRadius = 1f;
    [SerializeField] private LayerMask avoidCollisionLayerMask;


    private Action On_Death;
    private Action On_Hit;

    public Action OnDeath { get => On_Death; set => On_Death = value; }
    public Action OnHit { get => On_Hit; set => On_Hit = value; }

    private State state;
    void Awake() {
        state = State.FollowPath;
        currentHealth = maxHealth;
        if (followSplineContainer != null) {

            followSpline = followSplineContainer.Spline;
        }
        else {
            Debug.LogError($"Helicopter {gameObject.name} Spline not assigned");
        }
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        playerPosition = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        DecrementTimers();
        switch (state) {
            case State.FollowPath:
                FollowPath();
                break;
            case State.FollowPlayer:
                FollowPlayer();
                break;
            case State.ReturnToPath:
                ReturnToPath();
                break;
            default:
                break;
        }
    }

    private void DecrementTimers() {
        if (missileFireStepTimer > 0f) {
            missileFireStepTimer -= Time.fixedDeltaTime;
        }
        if (missileFireTimer > 0f) {
            missileFireTimer -= Time.fixedDeltaTime;
        }
        if (missileFireCooldownTimer > 0f) {
            missileFireCooldownTimer -= Time.fixedDeltaTime;
            if (missileFireCooldownTimer <= 0f) {
                missileFireTimer = missileFireDuration;
            }
        }

    }

    void FollowPath() {
        if (followSpline.Count <= 0)
            return;
        if (Vector3.SqrMagnitude(rb.position - playerPosition) <= playerDetectionRadius * playerDetectionRadius) {
            state = State.FollowPlayer;
            return;
        }
        SplineUtility.GetNearestPoint(followSpline, followSplineContainer.transform.InverseTransformPoint(rb.position), out float3 nearestPointFloat, out float closestT, resolution: 10, iterations: 5);
        Vector3 nearestPoint = new(nearestPointFloat.x, nearestPointFloat.y, nearestPointFloat.z);
        nearestPoint = followSplineContainer.transform.TransformPoint(nearestPoint);
        float movementDistance = movementSpeed * Time.fixedDeltaTime;
        if (Vector3.SqrMagnitude(transform.position - nearestPoint) >= movementDistance * movementDistance) {
            state = State.ReturnToPath;
            return;
        }
        //Enemy is near spline
        float movementT = movementDistance / followSplineContainer.CalculateLength();
        float newT = closestT + movementT;
        newT = Mathf.Repeat(newT, 1f);
        Vector3 splineTangent = followSpline.EvaluateTangent(newT);
        //Find point of newT
        Vector3 newPoint = followSpline.EvaluatePosition(newT);
        newPoint = followSplineContainer.transform.TransformPoint(newPoint);
        rb.MovePosition(newPoint);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(splineTangent), Time.fixedDeltaTime * rotationSpeed));
    }

    void FollowPlayer() {
        if (Vector3.SqrMagnitude(rb.position - playerPosition) >= playerDetectionRadiusFollowPlayer * playerDetectionRadiusFollowPlayer) {
            state = State.ReturnToPath;
            return;
        }
        //Move towards player horizontal position above flightMinHeight
        Vector3 playerHorizontalPosition = new(playerPosition.x, rb.position.y, playerPosition.z);
        Vector3 dirToPlayer = (playerHorizontalPosition - rb.position).normalized;
        float movementDistance = movementSpeed * Time.fixedDeltaTime;
        Vector3 newPoint = rb.position + dirToPlayer * movementDistance;
        Vector3 newPointHeightAdjusted = newPoint;

        if (Physics.Raycast(newPoint + Vector3.up, Vector3.down, out RaycastHit hit, flightMinHeight + 1f, ~avoidCollisionLayerMask, QueryTriggerInteraction.Ignore)) {
            newPointHeightAdjusted.y = hit.point.y + flightMinHeight;
            print($"Raycast Hit: {hit.collider.gameObject.name}");
        }
        else {
            newPointHeightAdjusted.y = rb.position.y; //If no ground found, keep current height
        }
        newPoint = Vector3.Lerp(newPoint, newPointHeightAdjusted, movementLerpSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPoint);
        //Rotate towards player
        Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));
        //Fire missiles at player
        if (missileFireCooldownTimer > 0f) {
            return; //Wait for cooldown
        }
        else if (missileFireTimer > 0f) {
            if (missileFireStepTimer <= 0f) {
                missileFireStepTimer = missileFireStep;
                if (missilePrefab != null) {
                    GameObject missile = Instantiate(missilePrefab, rb.position, Quaternion.identity);
                }
            }
        }
        else {
            missileFireCooldownTimer = missileFireCooldown;
        }

    }
    void ReturnToPath() {
        if (followSpline.Count <= 0)
            return;
        if (Vector3.SqrMagnitude(rb.position - playerPosition) <= playerDetectionRadius * playerDetectionRadius) {
            state = State.FollowPlayer;
            return;
        }
        if (Physics.SphereCast(rb.position, frontObstacleSphereCastRadius, transform.forward, out RaycastHit hit, frontObstacleDetectionDistance, ~avoidCollisionLayerMask, QueryTriggerInteraction.Ignore)) {
            //Stop moving until spherecast can return a null hit
            return;
        }
        SplineUtility.GetNearestPoint(followSpline, followSplineContainer.transform.InverseTransformPoint(rb.position), out float3 nearestPointFloat, out float closestT);
        //If nearest point is close then return to FollowPath state
        Vector3 nearestPoint = new(nearestPointFloat.x, nearestPointFloat.y, nearestPointFloat.z);
        nearestPoint = followSplineContainer.transform.TransformPoint(nearestPoint);
        float distanceToNearestPoint = Vector3.Distance(rb.position, nearestPoint);
        float movementDistance = movementSpeed * Time.fixedDeltaTime;
        if (distanceToNearestPoint < movementDistance) {
            state = State.FollowPath;
            return;
        }
        Vector3 splineTangent = followSpline.EvaluateTangent(closestT);
        Vector3 dirToPoint = (nearestPoint - transform.position).normalized;

        //Move towards nearest point on spline
        Vector3 newPoint = rb.position + dirToPoint * movementDistance;
        rb.MovePosition(newPoint);

        //Rotate towards spline, if enemy is going to reach spline within 1 second then start rotating towards spline tangent
        Quaternion targetRotation;
        Quaternion splineRotation = Quaternion.LookRotation(splineTangent);
        Quaternion dirToPointRotation = Quaternion.LookRotation(dirToPoint);
        float timeToReachSpline = Vector3.Distance(rb.position, nearestPoint) / movementSpeed;
        if (timeToReachSpline < 3f) {
            //Lerp between dirToPoint and splineTangent based on normalized time to reach spline
            timeToReachSpline = Mathf.Clamp01(timeToReachSpline / 3f);
            targetRotation = Quaternion.Lerp(dirToPointRotation, splineRotation, 1f - timeToReachSpline);
        }
        else {
            targetRotation = dirToPointRotation;
        }
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));
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
        return currentHealth / maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }


    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

}
