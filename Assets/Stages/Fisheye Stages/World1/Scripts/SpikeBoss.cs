using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SpikeBoss : MonoBehaviour
{
    [SerializeField] private AreaSpike areaSpike;
    [SerializeField] private ForwardSpike forwardSpike;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float waitTime = 0.5f;

    [SerializeField] private float attackTime = 3f;

    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float initialDelay = 0.5f;

    private Transform playerTransform;

    private bool isForwardAttack = false;
    private bool isAreaAttack = false;
    private bool hasAttackStarted = false;
    private bool isMoving = false;
    private bool isSlamming = false;
    private Vector3 positionToMoveTo;
    private void Start()
    {
        //find the player transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(InitialDelay());

        StartCoroutine(DeactivateSpikes());

        //start the coroutine to decide the first move
        StartCoroutine(DecideMove());
    }
    private void Update()
    {
        //make y rotation to face the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0; // Keep the y component zero to avoid tilting
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // Smoothly rotate towards the player
        }

        if (isForwardAttack && !hasAttackStarted)
        {
            hasAttackStarted = true; // Prevent multiple starts
            positionToMoveTo = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            isMoving = true; // Set moving to true for forward attack
        }
        else if (isAreaAttack && !hasAttackStarted)
        {
            hasAttackStarted = true; // Prevent multiple starts
            positionToMoveTo = new Vector3(playerTransform.position.x, transform.position.y + 15f, playerTransform.position.z);
            isMoving = true; // Set moving to true for area attack
        }

        if (isMoving)
        {
            transform.position = !isSlamming ? Vector3.MoveTowards(transform.position, positionToMoveTo, movementSpeed * Time.deltaTime) : Vector3.MoveTowards(transform.position, positionToMoveTo, movementSpeed * 3 * Time.deltaTime);

            // Check if the boss has reached the target position
            if (Vector3.Distance(transform.position, positionToMoveTo) < 0.1f)
            {
                if (isAreaAttack)
                {
                    if (isSlamming)
                    {
                        isSlamming = false;
                        isMoving = false;
                        StartCoroutine(StartAreaSpikeAttack());
                    }
                    else
                    {
                        // Move down to slam
                        positionToMoveTo = positionToMoveTo + new Vector3(0, -15f, 0);
                        isSlamming = true; // Set slamming to true to indicate the boss is slamming down
                    }
                }
                else if (isForwardAttack)
                {
                    isMoving = false;
                    StartCoroutine(StartForwardSpikeAttack());
                }
            }
        }
    }

    //make a co routine to decide which move to take
    public IEnumerator DecideMove()
    {
        //wait for a specified time before deciding the next move
        yield return new WaitForSeconds(waitTime);

        //randomly choose between area spike and forward spike
        int randomMove = Random.Range(0, 2);
        if (randomMove == 0)
        {
            isAreaAttack = true;
        }
        else
        {
            isForwardAttack = true;
        }
    }
    // A coroutine to deactivate the spikes after some time
    private IEnumerator DeactivateSpikes()
    {
        yield return new WaitForSeconds(0.2f); // wait for some time before deactivating the spikes

        areaSpike.gameObject.SetActive(false);
        forwardSpike.gameObject.SetActive(false);
    }

    private IEnumerator StartAreaSpikeAttack()
    {
        areaSpike.gameObject.SetActive(true);
        areaSpike.StartAttack(); // start the area spike attack
        yield return new WaitForSeconds(attackTime);
        areaSpike.gameObject.SetActive(false);

        isAreaAttack = false; // reset the area attack flag
        hasAttackStarted = false; // reset the attack started flag

        StartCoroutine(DecideMove()); // decide the next move after the area spike attack
    }

    private IEnumerator StartForwardSpikeAttack()
    {
        forwardSpike.gameObject.SetActive(true);
        forwardSpike.StartAttack(); // start the forward spike attack
        yield return new WaitForSeconds(attackTime);
        forwardSpike.gameObject.SetActive(false);

        isForwardAttack = false;
        hasAttackStarted = false; // reset the attack started flag

        StartCoroutine(DecideMove()); // decide the next move after the forward spike attack
    }
    
    private IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(initialDelay);
    }
}
