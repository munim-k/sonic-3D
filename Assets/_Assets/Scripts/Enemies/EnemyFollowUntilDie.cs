using System;
using UnityEngine;

public class EnemyFollowUntilDie : MonoBehaviour
{
    private bool isPlayerFound = false;
    private GameObject player;
    [Range(0f,1f)] [SerializeField] private float speed = 0.3f;
    [Range(0f,1f)] [SerializeField] private float rotationSpeed = 0.1f;

    private void Update()
    {
        if (isPlayerFound)
        {
            //only rotate y axis
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed);

            //dont move vertically
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y;
            //move towards player
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerFound = true;
            player = other.gameObject;
        }
    }
}
