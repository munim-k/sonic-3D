using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro.EditorUtilities;
using UnityEngine;

public class World3Boss : MonoBehaviour, BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int maxHealth = 200;
    private int currentHealth = 200;


    [SerializeField] private Transform flamethrowerPointCentre;
    [SerializeField] private AnimationCurve flamethrowerRotationCurve;
    [SerializeField] private float flamethrowerRotationSpeed = 1f;
    [SerializeField] private Flamethrower[] flameThrowers;
    [SerializeField] private float flamethrowerCooldown = 2f;
    [SerializeField] private float flamethrowerDuration = 5f;
    private float flamethrowerRotationLerp = 0f;
    private Quaternion flamethrowerRotationOriginal;

    [SerializeField] private Transform homingAttackPrefab;
    [SerializeField] private Transform[] homingAttackTransforms;
    [SerializeField] private float homingAttackCooldown = 2f;
    [SerializeField] private float homingAttackStep = 0.1f;
    [SerializeField] private float homingAttackDuration = 5f;
    private float homingAttackTimer = 0f;
    [SerializeField] private GameObject levelExit;



    private float attackTimer = 0f;
    private float attackCooldownTimer = 0f;

    public Action OnDamage;
    public Action<State> OnStateChange;

    private State state;
    public enum State
    {
        Flamethrower,
        HomingAttacks,
        Dead
    }

    void Awake()
    {
        currentHealth = maxHealth;
        state = State.Flamethrower;
        OnStateChange?.Invoke(state);
        flamethrowerRotationOriginal = flamethrowerPointCentre.localRotation;
        attackTimer = flamethrowerDuration;
        attackCooldownTimer = 0.1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Flamethrower:
                Flamethrower();
                break;
            case State.HomingAttacks:
                HomingAttacks();
                break;
            case State.Dead:
                break;
            default:
                break;
        }

    }

    private void Flamethrower()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f)
            {
                attackTimer = flamethrowerDuration;
                foreach (Flamethrower flamethrower in flameThrowers)
                {
                    flamethrower.ToggleFire(true);
                }
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                foreach (Flamethrower flamethrower in flameThrowers)
                {
                    flamethrower.ToggleFire(false);
                }
                attackCooldownTimer = flamethrowerCooldown;
                state = State.HomingAttacks;
                OnStateChange?.Invoke(state);
            }
        }

        //Spin the attack points
        flamethrowerRotationLerp += Time.fixedDeltaTime * flamethrowerRotationSpeed;
        if (flamethrowerRotationLerp > 1f)
        {
            flamethrowerRotationLerp = 0f;
        }
        Vector3 eulerAngles = flamethrowerRotationOriginal.eulerAngles;
        float l = flamethrowerRotationCurve.Evaluate(flamethrowerRotationLerp);
        flamethrowerPointCentre.transform.localRotation = Quaternion.Euler(0f, eulerAngles.y + (l * 360f), 0f);
    }

    private void HomingAttacks()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0f)
            {
                attackTimer = homingAttackDuration;
                homingAttackTimer = homingAttackStep;
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (homingAttackTimer > 0f)
            {
                homingAttackTimer -= Time.fixedDeltaTime;
            }
            if (homingAttackTimer <= 0f)
            {
                //Instantiate one homing attack on each transform
                foreach (Transform homingAttackTransform in homingAttackTransforms)
                {
                    Instantiate(homingAttackPrefab, homingAttackTransform.position, homingAttackTransform.rotation);
                }
                homingAttackTimer = homingAttackStep;
            }

            if (attackTimer <= 0f)
            {
                attackCooldownTimer = homingAttackCooldown;
                state = State.Flamethrower;
                OnStateChange?.Invoke(state);
            }
        }


    }





    public void DoDamageToEnemy(int damage)
    {
        if (state != State.HomingAttacks)
        {
            return;
        }
        currentHealth -= damage;
        OnDamage?.Invoke();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            state = State.Dead;
            levelExit.SetActive(true);
            OnStateChange?.Invoke(state);
        }
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth / maxHealth;
    }
}
