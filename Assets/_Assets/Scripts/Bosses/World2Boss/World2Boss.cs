using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class World2Boss : MonoBehaviour,BaseEnemy
{

    [SerializeField] private int maxHealth=100;
    private int currentHealth = 100;
    [SerializeField] private float attack1Cooldown;
    [SerializeField] private float attack1Duration;
    [SerializeField] private Transform attack1Transform;
    [SerializeField] private AnimationCurve attack1RotationCurve;
    [SerializeField] private float attack1RotationSpeed;
    private float attack1Lerp = 0f;
    private float attack1Timer = 0f;

    [SerializeField] private float attack2Cooldown;
    [SerializeField] private float attack2Duration;
    [SerializeField] private float attack2Step = 0.1f;
    [SerializeField] private float playerVelocityAdjustmentScaling = 0.5f;
    [SerializeField] private Transform attack2Projectiles;
    private float attack2Timer = 0f;
    private float attack2StepTimer = 0f;
    [SerializeField] private float stunDuration;


    [SerializeField] private World2BossCrystal[] crystals;

    private float attackCooldownTimer=0f;

    private float stunCooldownTimer=0f;
    public enum State
    {
        Idle,
        Attack1,
        Attack2,
        Stunned,
        Dead
    }
    private State state;

    public Action<State> OnStateChange;
    public Action OnDamage;

    private void Awake()
    {
        state = State.Idle;
        currentHealth = maxHealth;
        attackCooldownTimer = attack1Cooldown;
        InitializeCrystals();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Attack1:
                Attack1();
                break;
            case State.Attack2:
                Attack2();
                break;
            case State.Stunned:
                Stunned();
                break;
            case State.Dead:
                break;
            default:
                break;
        }

    }
    private void Idle()
    {
        if (attackCooldownTimer >= 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {

            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
            {
                state = State.Attack1;
                attack1Timer=attack1Duration;
                attack1Transform.gameObject.SetActive(true);
            }
            else
            {
                state = State.Attack2;
                attack2Timer=attack2Duration;
            }
         OnStateChange?.Invoke(state);
        }
    }
    private void Attack1()
    {
        if (attack1Timer >= 0f)
        {
            attack1Timer -= Time.fixedDeltaTime;
            attack1Lerp += Time.fixedDeltaTime * attack1RotationSpeed;
            attack1Lerp %= 1f;
            attack1Transform.rotation = Quaternion.Euler(0f, attack1RotationCurve.Evaluate(attack1Lerp) * 360f, 0f);
        }
        else
        {
            attack1Transform.gameObject.SetActive(false);
            state = State.Idle;
            attack1Lerp = 0f;
            attackCooldownTimer=attack1Cooldown;
        }

    }

    private void Attack2()
    {
        if (attack2Timer >= 0f)
        {
            attack2Timer -= Time.fixedDeltaTime;
            attack2StepTimer += Time.fixedDeltaTime;
            if (attack2StepTimer >= attack2Step)
            {
                attack2StepTimer = 0f;
                Vector3 pos = Player.CharacterInstance.playerBehaviourTree.playerTransform.position;
                pos += Player.CharacterInstance.playerBehaviourTree.moveVelocity * playerVelocityAdjustmentScaling;
                Transform projectile = Instantiate(attack2Projectiles, pos, Quaternion.identity);
                projectile.gameObject.SetActive(true);            
            }
        }
        else
        {
            state = State.Idle;
            attackCooldownTimer = attack2Cooldown;
        }
    }

    private void Stunned()
    {
        if (stunCooldownTimer >= 0f)
        {
            stunCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            state = State.Idle;
            OnStateChange?.Invoke(state);
            InitializeCrystals();
        }
    }

    public void StunBoss()
    {
        state = State.Stunned;
        OnStateChange?.Invoke(state);
        stunCooldownTimer= stunDuration;
    }

    private void InitializeCrystals()
    {
        foreach (var crystal in crystals)
        {
            crystal.gameObject.SetActive(true);
            crystal.SetCrystalMaterial(true);
        }
        //Activate a random crystal
        int randomIndex = UnityEngine.Random.Range(0, crystals.Length);
        crystals[randomIndex].SetCrystalMaterial(false);
    }
    public void DoDamageToEnemy(int damage)
    {
        if (state != State.Stunned)
            return;
        currentHealth -= damage;
        OnDamage?.Invoke();
        if (currentHealth <= 0)
        {
            state = State.Dead;
            OnStateChange?.Invoke(state);
        }
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth/ maxHealth;
    }
}
