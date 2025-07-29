using UnityEngine;

public class CannonEnemy : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject bombPrefab;
    public Transform firePoint;
    public float minFireForce = 10f;
    public float maxFireForce = 25f;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float fireRate = 2f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public float aimingThreshold = 5f; // Degrees difference before considered "aimed"

    private Transform player;
    private float nextFireTime;
    private bool isAimed = false;

    void Update()
    {
        if (player != null)
        {
            RotateTowardPlayer();
            
            if (isAimed && Time.time >= nextFireTime)
            {
                FireAtPlayer();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void RotateTowardPlayer()
    {
        if (player == null) return;

        // Calculate direction to player (ignoring Y axis)
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        // Create target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // Smoothly rotate toward player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Check if we're aimed close enough to the player
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        isAimed = angleDifference < aimingThreshold;
    }

    void FireAtPlayer()
    {
        if (bombPrefab == null || firePoint == null) return;

        // Calculate distance to player
        float distance = Vector3.Distance(firePoint.position, player.position);
        
        // Calculate dynamic force based on distance
        float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
        float dynamicForce = Mathf.Lerp(minFireForce, maxFireForce, normalizedDistance);

        // Create bomb
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            // Shoot straight forward (no upward angle)
            rb.AddForce(transform.forward * dynamicForce, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            isAimed = false;
        }
    }
}