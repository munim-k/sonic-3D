using System;
using System.Transactions;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    [SerializeField] private float movementSpeedHorizontal = 2f;
    [SerializeField] private float movementSpeedVertical = 1f;
    [SerializeField] private float flightMinHeight = 5f;
    [SerializeField] private GameObject deathExplosion;
    private Rigidbody rb;

    //FollowPath
    [SerializeField] private SplineContainer followSplineContainer;
    private Spline followSpline=null;
    [SerializeField] private float playerDetectionRadius = 10f;
    //FollowPlayer
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileFireStep = 0.2f;
    [SerializeField] private float missileFireDuration = 1f;
    [SerializeField] private float missileFireCooldown = 1f;
    private float missileFireStepTimer = 0f;
    private float missileFireTimer = 0f;

    //ReturnToPath
    [SerializeField] private float frontObstacleDetectionDistance = 5f;
    [SerializeField] private float frontObstacleSphereCastRadius = 1f;


    private Action On_Death;
    private Action On_Hit;

    public Action OnDeath { get => On_Death; set => On_Death = value; }
    public Action OnHit { get => On_Hit; set => On_Hit = value; }

    private State state;
    void Awake() {
        state = State.FollowPath;
        currentHealth = maxHealth;
        followSpline = followSplineContainer.Spline;
        rb= GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        switch (state) {
            case State.FollowPath:
                FollowPath();
                break;
            case State.FollowPlayer:
                break;
            case State.ReturnToPath:
                break;
            default:
                break;
        }
    }

    void FollowPath() {
        if (followSpline.Count <= 0)
            return;
        SplineUtility.GetNearestPoint(followSpline, followSplineContainer.transform.InverseTransformPoint(rb.position), out float3 nearestPointFloat, out float closestT);
        Vector3 nearestPoint = new(nearestPointFloat.x,nearestPointFloat.y, nearestPointFloat.z);
        //if (Vector3.SqrMagnitude(transform.position - nearestPoint) > 0.01f) {
        //    state = State.ReturnToPath;
        //    return;
        //}
        //Enemy is near spline
        Vector3 splineTangent = followSpline.EvaluateTangent(closestT);
        bool currentSplineDir = Vector3.Dot(transform.forward, splineTangent) > 0f;
        //Move forward in the currentSplineDir by movementDistance/splineLength
        float movementDistance = movementSpeedHorizontal * Time.fixedDeltaTime;
        float movementT = movementDistance / followSplineContainer.CalculateLength();
        float newT = closestT;
        if (currentSplineDir) {
            newT -= movementT;
        }
        else {
            newT += movementT;
        }
        newT = Mathf.Repeat(newT, 1f);
        //Find point of newT
        Vector3 newPoint = followSpline.EvaluatePosition(newT);
        newPoint = followSplineContainer.transform.TransformPoint(newPoint);
        rb.MovePosition(newPoint);

        if (!currentSplineDir) {
            splineTangent *= -1f;
        }
        rb.MoveRotation(Quaternion.LookRotation(splineTangent));
    }
  
    void IHittable.DoHit(int damage) {
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0) {
            OnDeath?.Invoke();
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

 
}
