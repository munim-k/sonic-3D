using UnityEngine;

public class CirclingPlane : MonoBehaviour
{
    [Header("Movement Settings")]
    public PlaneTriggerZone triggerZone;  // Assign the trigger zone object here
    public float circleSpeed = 1f;
    public float flightAltitude = 10f;
    public float bankingAmount = 30f;
    public float rotationSmoothness = 5f;

    [Header("Combat Settings")]
    public GameObject bombPrefab;
    public Transform bombDropPoint;
    public float minBombForce = 5f;
    public float maxBombForce = 15f;
    public float bombRate = 2f;
    public float bombAngleThreshold = 90f; // Wider bombing angle

    private float currentAngle;
    private float nextBombTime;
    private Transform player;
    private float circleRadius;
    private Vector3 lastPosition;

    void Start()
    {
        // Get radius from trigger zone
        if (triggerZone != null)
        {
            circleRadius = triggerZone.GetRadius();
        }
        else
        {
            circleRadius = 15f; // Default if not set
        }

        // Start at random position on circle
        currentAngle = Random.Range(0f, Mathf.PI * 2f);
        lastPosition = transform.position;
        UpdatePosition();
    }

    void Update()
    {
        lastPosition = transform.position;
        
        // Continue circling regardless of player presence
        currentAngle += circleSpeed * Time.deltaTime;
        UpdatePosition();

        // Calculate actual movement direction
        Vector3 actualMoveDirection = (transform.position - lastPosition).normalized;
        if (actualMoveDirection != Vector3.zero)
        {
            UpdateRotation(actualMoveDirection);
        }

        // Bombing logic
        if (player != null && Time.time >= nextBombTime && IsFacingPlayer())
        {
            DropBomb();
            nextBombTime = Time.time + bombRate;
        }
    }

    void UpdatePosition()
    {
        if (triggerZone == null) return;

        // Calculate new position
        Vector3 newPos = triggerZone.transform.position;
        newPos.x += Mathf.Sin(currentAngle) * circleRadius;
        newPos.z += Mathf.Cos(currentAngle) * circleRadius;
        newPos.y = flightAltitude;
        transform.position = newPos;
    }

    void UpdateRotation(Vector3 moveDirection)
    {
        // Calculate target rotation with proper plane orientation
        Quaternion targetRot = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(0, 90, 0);
        
        // Calculate banking based on turn sharpness
        float turnSharpness = circleSpeed / Mathf.Max(0.1f, circleRadius);
        float bankAngle = -Mathf.Clamp(turnSharpness * 30f, -bankingAmount, bankingAmount);
        
        // Apply rotation with banking
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot * Quaternion.Euler(0, 0, bankAngle),
            Time.deltaTime * rotationSmoothness
        );
    }

    void DropBomb()
    {
        if (bombPrefab == null || bombDropPoint == null || player == null) return;

        // Calculate direction to player (horizontal only)
        Vector3 direction = (player.position - bombDropPoint.position).normalized;
        direction.y = 0;

        // Calculate force based on horizontal distance only
        float horizontalDistance = Vector3.Distance(
            new Vector3(bombDropPoint.position.x, 0, bombDropPoint.position.z),
            new Vector3(player.position.x, 0, player.position.z)
        );
        float force = Mathf.Lerp(minBombForce, maxBombForce, horizontalDistance / (circleRadius * 2f));

        // Create and launch bomb
        GameObject bomb = Instantiate(bombPrefab, bombDropPoint.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    bool IsFacingPlayer()
    {
        if (player == null) return false;
        
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle < bombAngleThreshold;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}