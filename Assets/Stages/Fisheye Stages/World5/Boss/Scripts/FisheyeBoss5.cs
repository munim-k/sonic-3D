using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LightTransport;
using UnityEngine.UI;

public class FisheyeBoss5 : MonoBehaviour
{
    public Action OnLaserAttack;
    public Action OnSpikeAttack;
    public Action OnProjectileAttack;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private float attackShownWaitTime = 1f;
    [SerializeField] private float moveTime = 5f;
    [SerializeField] List<Material> attackMaterials = new List<Material>();
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldDisbaleTime = 5f;
    [SerializeField] private PhaseChange phaseChange;
    private int maxNumberOfAttacksBeforeStagger = 3;
    private bool isImmune = false;
    private Coroutine staggerCoroutine;
    private bool isStaggered = false;
    private float moveTimer = 0f;
    private bool isMoving = true;
    private GameObject player;
    private int phase = 1;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent.speed = speed;
        phaseChange.OnPhaseChange += PhaseChange_OnPhaseChange;

        StartCoroutine(DecideAttack(1));
    }

    private void PhaseChange_OnPhaseChange(int obj)
    {
        if(!isImmune)
            phase = obj;

        isImmune = true;

        if (phase == 4)
        {
            Destroy(gameObject);
            return;
        }

        if (isStaggered && staggerCoroutine != null)
        {
            StopCoroutine(staggerCoroutine);
        }

        isStaggered = false;
        shield.SetActive(true);
        agent.isStopped = false;

        StartCoroutine(DecideAttack(1));
    }

    private void Update()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveTime)
        {
            moveTimer = 0f;
            if (isMoving)
            {
                isMoving = false;
                agent.isStopped = true;
            }
            else
            {
                isMoving = true;
                agent.isStopped = false;
            }
        }

        if (isMoving)
        {
            Vector3 newPos = player.transform.position;
            newPos.y = transform.position.y;
            agent.SetDestination(newPos);
        }
    }

    private IEnumerator DecideAttack(int attackNumber)
    {
        yield return new WaitForSeconds(waitTime);

        int maxAttacks = 3;
        int attackCount = Mathf.Min(phase, maxAttacks);

        List<Action> allAttacks = new List<Action> { OnLaserAttack, OnSpikeAttack, OnProjectileAttack };
        List<Action> selectedAttacks = new List<Action>();

        while (selectedAttacks.Count < attackCount)
        {
            int randIndex = UnityEngine.Random.Range(0, allAttacks.Count);
            Action attack = allAttacks[randIndex];

            if (!selectedAttacks.Contains(attack))
            {
                selectedAttacks.Add(attack);
                attackMaterials[randIndex].SetFloat("_Emission", 2f);
            }
        }

        yield return new WaitForSeconds(attackShownWaitTime);

        foreach (Material mat in attackMaterials)
        {
            mat.SetFloat("_Emission", 0.5f);
        }

        foreach (Action action in selectedAttacks)
        {
            action?.Invoke();
        }
        if (attackNumber < maxNumberOfAttacksBeforeStagger)
        {
            StartCoroutine(DecideAttack(attackNumber + 1));
        }
        else
        {
            staggerCoroutine = StartCoroutine(Stagger());
            isImmune = false;
        }
    }

    private IEnumerator Stagger()
    {
        isStaggered = true;
        shield.SetActive(false);
        agent.isStopped = true;

        yield return new WaitForSeconds(shieldDisbaleTime);

        isStaggered = false;
        shield.SetActive(true);
        agent.isStopped = false;

        StartCoroutine(DecideAttack(1));
    }

    private void OnDestroy()
    {
        phaseChange.OnPhaseChange -= PhaseChange_OnPhaseChange;
    }
}
