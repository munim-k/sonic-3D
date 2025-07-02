using UnityEngine;

public class SpikeBallBoss : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private float forceOfMovement = 100f;

    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found in the scene!");
            return;
        }
    }

    private void Update()
    {
        if (player == null) return;

        //start moving toward the player
        Vector3 direction = (player.transform.position - transform.position).normalized;
        rb.AddForce(direction * forceOfMovement, ForceMode.Force);
    }
}
