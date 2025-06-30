using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBoss : MonoBehaviour
{
    private GameObject player;
    private GameObject playerInHitbox;

    [Header("Timing Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float throwForce = 50f;
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float delayBetweenAttacks = 2f;

    [SerializeField] private float homingTime = 5f;

    [Header("Game Objects")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject parentOfHoming;
    private Vector3 swordPosition;
    private Quaternion swordRotation;
    [SerializeField] private List<HomingSingle> homingSingles = new List<HomingSingle>();
    private List<Vector3> homingSinglePositions = new List<Vector3>();
    private List<Quaternion> homingSingleRotations = new List<Quaternion>();

    private bool isSwordAttack = false;
    private float swordTimer = 0f;
    private void Start()
    {
        // Find the player GameObject in the scene.
        player = GameObject.FindGameObjectWithTag("Player");

        swordPosition = sword.transform.localPosition;
        swordRotation = sword.transform.localRotation;
        sword.SetActive(false);

        for (int i = 0; i < homingSingles.Count; i++)
        {
            homingSinglePositions.Add(homingSingles[i].transform.localPosition);
            homingSingleRotations.Add(homingSingles[i].transform.localRotation);
            homingSingles[i].gameObject.SetActive(false);
        }

        StartCoroutine(InitialDelay());

        StartCoroutine(DecideAttack());
    }

    private IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(initialDelay);
    }
    private IEnumerator ThrowPlayer()
    {
        if (playerInHitbox != null)
        {
            Rigidbody playerRigidbody = playerInHitbox.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Calculate throw direction with upward arc
                Vector3 throwDirection = playerInHitbox.transform.position - transform.position;
                throwDirection.y = 0.5f; // Add vertical force to create a proper throw arc
                playerRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }
        yield return null;
    }

    private IEnumerator DecideAttack()
    {
        yield return new WaitForSeconds(delayBetweenAttacks);

        // Randomly choose between throwing the player or spawning homing singles
        if (Random.value < 0.5f)
        {
            //start sword attack
            sword.transform.localPosition = swordPosition;
            sword.transform.localRotation = swordRotation;
            sword.SetActive(true);

            //randomly teleport near the player
            Vector3 randomPosition = player.transform.position + Random.insideUnitSphere * 15f;
            randomPosition.y = transform.position.y; // Keep the y position the same
            transform.position = randomPosition;

            isSwordAttack = true;
        }
        else
        {
            StartCoroutine(ThrowPlayer());

            StartCoroutine(SpawnHomingSingles());
        }

        // Restart the attack decision after a delay
        StartCoroutine(DecideAttack());
    }

    private IEnumerator SpawnHomingSingles()
    {
        for (int i = 0; i < homingSinglePositions.Count; i++)
        {
            GameObject instance = Instantiate(homingSingles[i].gameObject, parentOfHoming.transform);
            instance.transform.localPosition = homingSinglePositions[i];
            instance.transform.localRotation = homingSingleRotations[i];
            instance.transform.SetParent(null);

            HomingSingle homing = instance.GetComponent<HomingSingle>();
            if (homing != null)
            {
                homing.gameObject.SetActive(true);
                homing.shouldStart = true;
                homing.homingTime = homingTime;
                homing.timer = 0f;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    private void Update()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; // Keep the y component zero to avoid tilting
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // Smoothly rotate towards the player
        }

        if (isSwordAttack)
        {
            swordTimer += Time.deltaTime;
            if (swordTimer >= delayBetweenAttacks)
            {
                isSwordAttack = false;
                sword.SetActive(false);
                swordTimer = 0f;
            }
            //rotate the sword from -75 to 75 degrees around the y-axis
            float rotationAngle = Mathf.PingPong(Time.time * 200, 150) - 75; // Adjust the speed and range of the rotation
            sword.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInHitbox = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInHitbox = null;
        }
    }
}
