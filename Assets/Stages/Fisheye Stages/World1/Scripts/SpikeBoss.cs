using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SpikeBoss : MonoBehaviour
{
    [SerializeField] private AreaSpike areaSpike;
    [SerializeField] private ForwardSpike forwardSpike;
    [SerializeField] private float movementDistance = 10f;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float waitTime = 2f;

    [SerializeField] private float attackTime = 3f;

    private bool isMoving = false;
    private Vector3 oldPosition;
    private int randomMovement;

    private void Start()
    {
        StartCoroutine(DeactivateSpikes());

        //start the coroutine to decide the first move
        StartCoroutine(DecideMove());
    }
    private void Update()
    {

        if (isMoving)
        {
            //move the boss based on the random number
            if (randomMovement == 0)
            {
                //move left
                transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
            }
            else if (randomMovement == 1)
            {
                //move right
                transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
            }
            else if (randomMovement == 2)
            {
                //move up
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else if (randomMovement == 3)
            {
                //move down
                transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
            }
            //check if the boss has moved the movement distance
            if (Vector3.Distance(oldPosition, transform.position) >= movementDistance)
            {
                //stop moving
                isMoving = false;
                oldPosition = transform.position; //update the old position to the current position
                StartCoroutine(DecideMove()); //decide the next move
            }
        }
    }

    //make a co routine to decide which move to take
    public IEnumerator DecideMove()
    {
        //wait for a specified time before deciding the next move
        yield return new WaitForSeconds(waitTime);

        //randomly choose between area spike and forward spike
        int randomMove = Random.Range(0, 3);
        if (randomMove == 0)
        {
            areaSpike.gameObject.SetActive(true);
            areaSpike.StartAttack(); // start the area spike attack
            yield return new WaitForSeconds(attackTime);
            areaSpike.gameObject.SetActive(false);

            StartCoroutine(DecideMove()); // decide the next move after the area spike attack
        }
        else if (randomMove == 1)
        {
            //activate forward spike
            forwardSpike.gameObject.SetActive(true);
            forwardSpike.StartAttack(); // start the forward spike attack
            yield return new WaitForSeconds(attackTime);
            forwardSpike.gameObject.SetActive(false);

            StartCoroutine(DecideMove()); // decide the next move after the forward spike attack
        }
        else
        {
            oldPosition = transform.position;
            isMoving = true;
            randomMovement = Random.Range(0, 4); // Randomly choose a direction to move
        }
    }
    // A coroutine to deactivate the spikes after some time
    private IEnumerator DeactivateSpikes()
    {
        yield return new WaitForSeconds(0.2f); // wait for some time before deactivating the spikes

        areaSpike.gameObject.SetActive(false);
        forwardSpike.gameObject.SetActive(false);
    }
}
