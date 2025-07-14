using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FisheyeBoss4 : MonoBehaviour
{
    public event Action OnSpikeSlamAttackChosen;

    public enum Attacks
    {
        None,
        LaserAttack,
        SpikeThrow,
        SpikeSlam
    }

    private Attacks currentAttack = Attacks.None;
    [SerializeField] private float waitTime = 1.5f;
    private int maxRandNumber = 2;
    [SerializeField] private float showAttackSeconds = 0.4f;
    [SerializeField] private PhaseChange phaseChange;

    [Header("Phase activating Objects")]
    [SerializeField] private List<GameObject> phaseChangeGameObjects;
    [SerializeField] List<Transform> phaseSpawnPoints;
    [SerializeField] private float moveSpeed = 35f;
    private bool isMoving = false;
    private Vector3 direction;

    [Header("Laser Attack Variables")]
    [SerializeField] private float laserSpeed = 150f;
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float maxRotationAngle = 45f;
    private Vector3 origianlRotation;
    private float singleWaveTime;
    private float laserStartTime;
    private Vector3 laserPos;
    private Vector3 laserScale;

    [Header("SpikeThrow Objects")]
    [SerializeField] private GameObject spikeThrow;
    [SerializeField] private float spikeThrowSpeed = 50f;
    [SerializeField] private float spikeThrowTime = 1.5f;
    private float spikeThrowTimer = 0f;
    private Vector3 spikeThrowPos;
    private int currentSpawnPoint;

    [Header("Spike Slam Objects")]
    [SerializeField] private SpikesAnimation spikesAnimation;
    [SerializeField] private GameObject spikeSlamObject;

    private void Start()
    {
        laserPos = laserObject.transform.localPosition;
        laserScale = laserObject.transform.localScale;
        origianlRotation = transform.eulerAngles;
        laserObject.SetActive(false);

        spikeThrow.SetActive(false);
        spikeThrowPos = spikeThrow.transform.localPosition;

        StartCoroutine(DecideAttack());

        phaseChange.OnPhaseChange += PhaseChange_OnPhaseChange;
        spikesAnimation.OnSpikeAnimationComplete += SpikesAnimation_OnAnimationComplete;
    }

    private void SpikesAnimation_OnAnimationComplete()
    {
        StartCoroutine(DecideAttack());
    }

    private void PhaseChange_OnPhaseChange(int phase)
    {
        if (phase == 7)
        {
            Destroy(gameObject);
            return;
        }

        if (phase == 2 || phase == 3)
        {
            maxRandNumber++;
            waitTime -= 0.5f;
        }

        phaseChangeGameObjects[phase - 2].SetActive(true);
        isMoving = true;
        direction = phaseSpawnPoints[phase - 2].position - transform.position;
        currentSpawnPoint = phase - 2;
        Debug.Log(currentSpawnPoint);
    }

    private void Update()
    {
        if (currentAttack == Attacks.LaserAttack)
        {
            //rotate the enemy
            float elapsed = Time.time - laserStartTime;
            float angle = Mathf.Sin(elapsed * rotationSpeed) * maxRotationAngle;
            transform.rotation = Quaternion.Euler(origianlRotation.x, origianlRotation.y + angle, origianlRotation.z);

            // increase scale of laser
            laserObject.transform.localScale += new Vector3(0f, 0f, laserSpeed * Time.deltaTime);
            laserObject.transform.localPosition += Vector3.forward * laserSpeed * Time.deltaTime;
            if (elapsed >= singleWaveTime)
            {
                currentAttack = Attacks.None;
                transform.eulerAngles = origianlRotation;
                laserObject.SetActive(false);

                spikeSlamObject.SetActive(true);

                StartCoroutine(DecideAttack());
            }
        }
        else if (currentAttack == Attacks.SpikeThrow)
        {
            //move the spikes in the forward direction
            spikeThrowTimer += Time.deltaTime;
            spikeThrow.transform.localPosition += new Vector3(0f, 0f, spikeThrowSpeed * Time.deltaTime);
            if (spikeThrowTimer >= spikeThrowTime)
            {
                currentAttack = Attacks.None;
                StartCoroutine(DecideAttack());
                spikeThrowTimer = 0f;
            }
        }

        if (isMoving)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            direction = phaseSpawnPoints[currentSpawnPoint].position - transform.position;
            if (Vector3.Distance(transform.position, phaseSpawnPoints[currentSpawnPoint].position) <= 0.05f)
            {
                isMoving = false;
            }
        }
    }

    private IEnumerator DecideAttack()
    {
        yield return new WaitForSeconds(waitTime);
        int randNumber = UnityEngine.Random.Range(1, maxRandNumber);

        if (randNumber == 1)
        {
            //Laser Attack
            laserObject.transform.localPosition = laserPos;
            laserObject.transform.localScale = laserScale;
            laserObject.SetActive(true);
            //deactive spike slam so it doesnt appear
            spikeSlamObject.SetActive(false);

            yield return new WaitForSeconds(showAttackSeconds);
            singleWaveTime = 2 * Mathf.PI / rotationSpeed;
            currentAttack = Attacks.LaserAttack;
            laserStartTime = Time.time;
        }
        else if (randNumber == 2)
        {
            spikeThrow.SetActive(true);

            int newRand = UnityEngine.Random.Range(1, 3);
            spikeThrow.transform.localPosition = spikeThrowPos;
            if (newRand == 1)
            {
                spikeThrow.transform.localPosition += new Vector3(0f, 1.5f, 0f);
            }
            spikeThrow.SetActive(true);
            yield return new WaitForSeconds(showAttackSeconds * 2);

            currentAttack = Attacks.SpikeThrow;
        }
        else
        {
            OnSpikeSlamAttackChosen?.Invoke();
        }
    }
}
