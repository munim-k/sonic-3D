using UnityEngine;


public class World4BossMovement : MonoBehaviour {
    private float attackDuration;
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1f;

    public bool IsMoving { get; private set; } = true;
    private float rotation;
    private float attackTimer = 0f;
    private void Start() {
        target = Player.CharacterInstance.playerBehaviourTree.modelTransform;
    }
    public void StartMoving() {
        IsMoving = true;
        attackTimer = attackDuration;
    }

    public void SetAttackDuration(float attackDuration) {
        this.attackDuration = attackDuration;
    }

    void FixedUpdate() {
        // Movement handling

        if (target != null && IsMoving) {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f) {
                IsMoving = false;
                return;
            }
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            float distance = direction.magnitude;
            float speed = distance / attackTimer;
            if (distance > stoppingDistance) {
                transform.position = direction.normalized * speed * Time.fixedDeltaTime;
            }
            rotation = rotationSpeed * Time.fixedDeltaTime;
        }
        else {
            rotation = Mathf.Lerp(rotation, 0, Time.fixedDeltaTime * 2f);
        }
        transform.rotation = transform.rotation * Quaternion.Euler(0, rotation, 0);
    }
}
