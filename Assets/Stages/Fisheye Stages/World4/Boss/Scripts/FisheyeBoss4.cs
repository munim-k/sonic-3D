using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FisheyeBoss4 : MonoBehaviour
{
    public event Action OnSpikeAttackChosen;

    [Header("Phase specifc Variables")]
    [Header("Phase 2 GameObjects")]
    [SerializeField] private List<GameObject> phase2ActivatingObjects;
    [SerializeField] private Transform phase2SpawnPosition;
    [Header("Phase 3 GameObjects")]
    [SerializeField] private List<GameObject> phase3ActivatingObjects;
    [SerializeField] private Transform phase3SpawnPosition;


    [SerializeField] private BossHitbox bossHitbox;

    [Header("Variables Related to the laser Attack")]
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float laserTime = 2f;
    [SerializeField] private float laserSpeed = 100f;
    private float laserTimer = 0f;
    private Vector3 laserPos;
    private Vector3 laserScale;
    private bool isLaserAttack = false;


    [Header("Variables related to spike attack")]
    [SerializeField] private SpikesAnimation spikes;

    [Header("Spike Throw Objects")]
    [SerializeField] private GameObject spikeThrow;
    [SerializeField] private float spikeThrowTime = 2f;
    [SerializeField] private float spikeThrowSpeed = 100f;
    private float spikeThrowTimer = 0f;
    private Vector3 spikeThrowPos;
    private bool isSpikeThrowAttack = false;

    [Header("Normal Variables")]
    [SerializeField] private float waitTime = 2f;

    private bool shouldDoSpikeSlamAttack = false;
    private void Start()
    {
        StartCoroutine(WaitInitially());
        laserPos = laserObject.transform.localPosition;
        laserScale = laserObject.transform.localScale;
        laserObject.SetActive(false);

        spikeThrow.SetActive(false);
        spikeThrowPos = spikeThrow.transform.localPosition;

        spikes.OnSpikeAnimationComplete += Spikes_OnSpikeAnimationComplete;
        bossHitbox.OnPhaseChange += BossHitbox_OnPhaseChange;
    }

    private void BossHitbox_OnPhaseChange(int phase)
    {
        shouldDoSpikeSlamAttack = false;
        if (phase == 4)
        {
            Destroy(gameObject);
        }
        else if (phase == 2)
        {
            foreach (GameObject obj in phase2ActivatingObjects)
            {
                obj.SetActive(true);
            }
            transform.position = phase2SpawnPosition.position;

            isLaserAttack = false;
            isSpikeThrowAttack = false;
            spikeThrow.SetActive(false);
            laserObject.SetActive(false);

            StopAllCoroutines();

            StartCoroutine(DecideAttack());
        }
        else
        {
            foreach (GameObject obj in phase3ActivatingObjects)
            {
                obj.SetActive(true);
            }
            transform.position = phase3SpawnPosition.position;

            isLaserAttack = false;
            isSpikeThrowAttack = false;

            StopAllCoroutines();

            StartCoroutine(DecideAttack());
        }
    }

    private void Spikes_OnSpikeAnimationComplete()
    {
        StartCoroutine(DecideAttack());
    }

    private void Update()
    {
        if (isLaserAttack)
        {
            laserTimer += Time.deltaTime;
            if (laserTimer >= laserTime)
            {
                isLaserAttack = false;
                laserTimer = 0f;
                laserObject.transform.localPosition = laserPos;
                laserObject.transform.localScale = laserScale;
                laserObject.SetActive(false);

                StartCoroutine(DecideAttack());
            }

            laserObject.transform.localScale += new Vector3(Mathf.Abs(transform.forward.x) * laserSpeed * Time.deltaTime, 0f, Mathf.Abs(transform.forward.z) *  laserSpeed * Time.deltaTime);
            laserObject.transform.localPosition -= new Vector3(transform.forward.x * laserSpeed / 2f * Time.deltaTime, 0f, transform.forward.z * laserSpeed / 2f * Time.deltaTime);
        }
        else if (isSpikeThrowAttack)
        {
            spikeThrowTimer += Time.deltaTime;
            if (spikeThrowTimer >= spikeThrowTime)
            {
                isSpikeThrowAttack = false;
                spikeThrowTimer = 0f;
                spikeThrow.transform.localPosition = spikeThrowPos;
                spikeThrow.SetActive(false);

                StartCoroutine(DecideAttack());
            }
            spikeThrow.transform.localPosition -= new Vector3(transform.forward.x * spikeThrowSpeed * Time.deltaTime, 0f, transform.forward.z * spikeThrowSpeed * Time.deltaTime);
        }
    }


    private IEnumerator DecideAttack()
    {
        yield return new WaitForSeconds(waitTime);
        int random = UnityEngine.Random.Range(1, shouldDoSpikeSlamAttack ? 5 : 3);
        if (random == 1)
        {
            isLaserAttack = true;
            laserObject.SetActive(true);
        }
        else if (random >= 3)
        {
            OnSpikeAttackChosen?.Invoke();
        }
        else
        {
            spikeThrow.SetActive(true);
            int newRand = UnityEngine.Random.Range(1, 3);
            if (newRand == 2)
            {
                spikeThrow.transform.localPosition += new Vector3(0f, 2f, 0f);
            }
            StartCoroutine(ShowSpikeThrowAttack());
        }
    }
    private IEnumerator WaitInitially()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(DecideAttack());
    }

    private IEnumerator ShowSpikeThrowAttack()
    {
        yield return new WaitForSeconds(0.3f);
        isSpikeThrowAttack = true;
    }

    private void OnDestroy()
    {
        spikes.OnSpikeAnimationComplete -= Spikes_OnSpikeAnimationComplete;
        bossHitbox.OnPhaseChange -= BossHitbox_OnPhaseChange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            shouldDoSpikeSlamAttack = true;
            Debug.Log("Hello");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            shouldDoSpikeSlamAttack = false;
        }
    }
}
