using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;
public class FisheyeBoss6Attacks : MonoBehaviour
{
    [SerializeField] private GameObject poisonBottlePrefab;
    [SerializeField] private GameObject bloodBottlePrefab;
    [SerializeField] private GameObject deathBottlePrefab;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private Transform bottleSpawnPoint;
    [SerializeField] private NavMeshAgent agent;
    private GameObject player;

    public Action OnAttack;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DecideAttack());
    }
    private void Update()
    {
        //face the player always
        if (player != null)
        {
            agent.SetDestination(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }
    }

    private IEnumerator DecideAttack()
    {
        while (true)
        {
            int randomAttack = UnityEngine.Random.Range(0, 3);

            switch (randomAttack)
            {
                case 0:
                    SpawnBottle(poisonBottlePrefab);
                    break;
                case 1:
                    SpawnBottle(bloodBottlePrefab);
                    break;
                case 2:
                    SpawnBottle(deathBottlePrefab);
                    break;
            }
            OnAttack?.Invoke();
            
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnBottle(GameObject bottlePrefab)
    {

        GameObject newBottle = Instantiate(bottlePrefab, bottleSpawnPoint.position, Quaternion.identity);
        Rigidbody bottleRigidBody = newBottle.GetComponent<Rigidbody>();
        if (bottleRigidBody != null)
        {
            Vector3 start = bottleSpawnPoint.position;

            // Add random offset to the target near the player
            Vector3 playerPos = player.transform.position;
            float range = 3f;
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-range, range), 0, UnityEngine.Random.Range(-range, range));
            Vector3 target = playerPos + randomOffset;

            float arcHeight = 3f;
            float gravity = Mathf.Abs(Physics.gravity.y);

            Vector3 displacement = target - start;
            Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
            float distance = horizontalDisplacement.magnitude;
            float heightDifference = displacement.y;

            float timeUp = Mathf.Sqrt(4 * arcHeight / gravity);
            float timeDown = Mathf.Sqrt(4 * Mathf.Max(0.01f, arcHeight - heightDifference) / gravity);
            float totalTime = timeUp + timeDown;

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * arcHeight);
            Vector3 velocityXZ = horizontalDisplacement / totalTime;

            Vector3 finalVelocity = velocityXZ + velocityY;

            bottleRigidBody.linearVelocity = finalVelocity;
        }
    }
}