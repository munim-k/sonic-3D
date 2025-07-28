using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class FisheyeBoss7 : MonoBehaviour
{
    [SerializeField] private GameObject flame1;
    [SerializeField] private GameObject flame2;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missileSpawnPosition1;
    [SerializeField] private Transform missileSpawnPosition2;
    [SerializeField] private float flameAttackTime = 5f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float missileWaitTime = 3f;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed = 100f;
    private bool isDoingFlameAttack = false;
    private float flameAttackTimer = 0f;
    private GameObject player;
    private bool shouldFacePlayer = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DecideAttack());
    }

    private void Update()
    {
        agent.SetDestination(player.transform.position);
        if (shouldFacePlayer)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            direction.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        if (isDoingFlameAttack)
        {
            flameAttackTimer += Time.deltaTime;
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

            if (flameAttackTimer >= flameAttackTime)
            {
                flame1.SetActive(false);
                flame2.SetActive(false);

                isDoingFlameAttack = false;
                flameAttackTimer = 0f;
                shouldFacePlayer = true;

                agent.updateRotation = true;

                StartCoroutine(DecideAttack());
            }
        }
    }

    private IEnumerator DecideAttack()
    {
        yield return new WaitForSeconds(waitTime);

        int random = UnityEngine.Random.Range(0, 2);
        if (random == 0)
        {
            flame1.SetActive(true);
            flame2.SetActive(true);

            shouldFacePlayer = false;

            isDoingFlameAttack = true;
            agent.updateRotation = false;
        }
        else
        {
            //do missile attack
            Instantiate(missilePrefab, missileSpawnPosition1).transform.parent = null;
            Instantiate(missilePrefab, missileSpawnPosition2).transform.parent = null;

            StartCoroutine(WaitForMissileDelay());
        }
    }

    private IEnumerator WaitForMissileDelay()
    {
        yield return new WaitForSeconds(missileWaitTime);
        StartCoroutine(DecideAttack());
    }
}
