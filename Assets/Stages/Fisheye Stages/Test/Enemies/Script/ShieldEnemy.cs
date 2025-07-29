using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShieldEnemy : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> patrolPoints;
    private int currentPoint = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;
            agent.speed *= 1.5f;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            if (patrolPoints.Count == 0)
                return;
            agent.SetDestination(new Vector3(patrolPoints[currentPoint].position.x, transform.position.y, patrolPoints[currentPoint].position.z));

            if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                currentPoint++;
            }
        }
        else
        {
            agent.SetDestination(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }
    }
}
