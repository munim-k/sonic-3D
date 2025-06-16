using UnityEngine;
using System;


public class World4BossMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float stoppingDistance = 1f;

    public bool IsMoving { get; private set; } = true;

    public void SetMovement(bool isMoving)
    {
        IsMoving = isMoving;
    }

    void Update()
    {
        // Movement handling
        if (target != null && IsMoving)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep movement on the horizontal plane
            float distance = direction.magnitude;

            // Rotate towards the target
            if (distance > stoppingDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Move towards the target
                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
    }
}