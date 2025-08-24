using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PatrollerBotEnemy : MonoBehaviour, BaseEnemy, IHittable {
    public enum State {
        Moving,
        Turning
    }

    [SerializeField] private int maxHealth = 100;
    private int currentHealth=0;
    [SerializeField] private GameObject deathExplosion;
    //Moving
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float forwardRayDistance = 1f;
    [SerializeField] private float forwardRayRadius = 0.5f;
    [SerializeField] private Vector3 forwardRayOffset;
    [SerializeField] private float downwardsRayDistance = 1f;
    [SerializeField] private float downwardsRayRadius = 0.5f;
    [SerializeField] private Vector3 downwardsRayOffset;
    [SerializeField] private LayerMask raycastLayersToAvoid;

    //Turning
    [SerializeField] private float turnSpeed = 10f; //Degrees per second    
    private Vector3 oldLocalRotation;
    private Vector3 newLocalRotation;
    private Action On_Death;
    private Action On_Hit;

    Action BaseEnemy.OnDeath { get => On_Death; set => On_Death = value; }
    Action IHittable.OnHit { get => On_Hit; set => On_Hit = value; }

    private State state;
    public Action<State> OnStateChange;
    private Rigidbody rb;
    private void Start() {
        currentHealth= maxHealth;
        rb = GetComponent<Rigidbody>();
        state = State.Moving;
        OnStateChange?.Invoke(state);
    }
    private void FixedUpdate() {
        switch (state) {
            case State.Moving:
                Moving();
                break;
            case State.Turning:
                Turning();
                break;
        }
    }

    private void Moving() {
        //Do two raycasts, one facing transform.forward from the middle and one facing downwards to detect ground
        //If forward raycast hits something or downwards raycast doesn't hit anything, switch to turning state
        Vector3 forwardRayOrigin = transform.position + transform.rotation* forwardRayOffset;
        Vector3 downwardsRayOrigin = transform.position + transform.rotation* downwardsRayOffset;
        Debug.DrawRay(forwardRayOrigin, transform.forward * forwardRayDistance, Color.red);
        if (Physics.SphereCast(forwardRayOrigin, forwardRayRadius,transform.forward, out RaycastHit forwardHit, forwardRayDistance, ~raycastLayersToAvoid, QueryTriggerInteraction.Ignore)) {
          //  print($"Forward hit: {forwardHit.collider.gameObject.name}");
            TransitionToTurning();
            return;
        }
        Debug.DrawRay(downwardsRayOrigin, Vector3.down * downwardsRayDistance, Color.blue);
        if (!Physics.SphereCast(downwardsRayOrigin, downwardsRayRadius, Vector3.down, out RaycastHit downwardsHit, downwardsRayDistance, ~raycastLayersToAvoid, QueryTriggerInteraction.Ignore)) {
         //   print("No ground detected");
            TransitionToTurning();
            return;
        }
        //Move forward
        rb.MovePosition(rb.position + (moveSpeed * Time.fixedDeltaTime * transform.forward));
    }
    private void TransitionToTurning() {
        oldLocalRotation = transform.localEulerAngles;
        newLocalRotation = Quaternion.Euler(oldLocalRotation + new Vector3(0, 180f, 0)).eulerAngles;
        rb.isKinematic = true;
        state = State.Turning;
        OnStateChange?.Invoke(state);

    }
    private void Turning() {
        //Rotate towards newLocalRotation
        Vector3 currentLocalRotation = transform.localEulerAngles;
        Vector3 targetLocalRotation = newLocalRotation;
        float step = turnSpeed * Time.fixedDeltaTime;
        Vector3 nextLocalRotation = Vector3.MoveTowards(currentLocalRotation, targetLocalRotation, step);
        nextLocalRotation= Quaternion.Euler(nextLocalRotation).eulerAngles; 
        transform.localEulerAngles = nextLocalRotation;
        //If close enough to newLocalRotation, switch to moving state
        if (Vector3.Distance(nextLocalRotation, targetLocalRotation) < 0.1f) {
            rb.isKinematic = false;
            state = State.Moving;
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
            Destroy(gameObject);
        }
        else {
            if (state == State.Moving) {
                TransitionToTurning();
            }
        }
    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)currentHealth / (float)maxHealth;
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }

}
