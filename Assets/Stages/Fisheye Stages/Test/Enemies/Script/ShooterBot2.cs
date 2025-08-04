using UnityEngine;

public class ShooterBot2 : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints = new Transform[3];
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] private Transform modelToRotate; // The visual model (optional)

    private bool playerInside = false;
    private float fireTimer = 0f;
    private Transform player;

    private void Update()
    {
        if (!playerInside || player == null) return;

        // Rotate only on Y-axis toward player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            modelToRotate.rotation = Quaternion.Slerp(modelToRotate.rotation, targetRotation, Time.deltaTime * 5f);
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            ShootAll();
            fireTimer = fireDelay;
        }
    }

    private void ShootAll()
    {
        foreach (Transform firePoint in firePoints)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            proj.GetComponent<ShooterBotProjectile>().SetLaunchDir(firePoint.forward * 5f); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
            fireTimer = 0f; // Fire immediately
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }
}
