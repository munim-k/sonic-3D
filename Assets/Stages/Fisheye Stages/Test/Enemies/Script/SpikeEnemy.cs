using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SpikeEnemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }
}
