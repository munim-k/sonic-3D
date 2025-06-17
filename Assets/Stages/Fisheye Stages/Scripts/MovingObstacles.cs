using System;
using BehaviourGraph;
using UnityEngine;

public class MovingObstacles : MonoBehaviour
{
    public enum ObstacleType
    {
        Horizontal,
        Vertical,
        Forward
    }
    [Range(0.01f, 0.3f)][SerializeField] private float speed = .1f;
    [Range(1, 30)][SerializeField] private float distance = 5f;
    [SerializeField] private ObstacleType direction;
    [SerializeField] private bool isMovingTowardsMax = false;

    private Vector3 startPosition;
    private Vector3 minPosition;
    private Vector3 maxPosition;

    private void Start()
    {
        startPosition = transform.position;

        switch (direction)
        {
            case ObstacleType.Horizontal:
                maxPosition = startPosition + new Vector3(distance, 0, 0);
                minPosition = startPosition - new Vector3(distance, 0, 0);
                break;
            case ObstacleType.Vertical:
                maxPosition = startPosition + new Vector3(0, distance, 0);
                minPosition = startPosition - new Vector3(0, distance, 0);
                break;
            case ObstacleType.Forward:
                maxPosition = startPosition + new Vector3(0, 0, distance);
                minPosition = startPosition - new Vector3(0, 0, distance);
                break;
            default:
                Debug.LogError("Invalid ObstacleType specified.");
                return;
        }
    }

    private void FixedUpdate()
    {
        if (isMovingTowardsMax)
        {
            transform.position = Vector3.MoveTowards(transform.position, maxPosition, speed);
            if (Vector3.Distance(transform.position, maxPosition) < 0.01f)
            {
                isMovingTowardsMax = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, minPosition, speed);
            if (Vector3.Distance(transform.position, minPosition) < 0.01f)
            {
                isMovingTowardsMax = true;
            }
        }
    }
}
