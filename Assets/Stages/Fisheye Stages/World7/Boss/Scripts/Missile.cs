using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    [SerializeField] private float maxFollowTime = 7.5f;    
    [SerializeField] private float timer = 0f;
    [SerializeField] private float maxDistanceBeforeExplode = 0.1f;
    [SerializeField] private GameObject bombParticle;
    private bool isInInitialPhase = true;
    private Transform player;
    private float initialPhaseTime = 1f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found in scene.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (isInInitialPhase)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= initialPhaseTime)
            {
                timer = 0f;
                isInInitialPhase = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= maxFollowTime)
            {
                Instantiate(bombParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            // Move towards player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Rotate to face player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, player.transform.position) <= maxDistanceBeforeExplode)
            {
                Instantiate(bombParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
