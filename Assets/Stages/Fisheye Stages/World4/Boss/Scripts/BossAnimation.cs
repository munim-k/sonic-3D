using System;
using System.Collections;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    [SerializeField] private FisheyeBoss4 boss;
    private Animator animator;

    public event Action OnBossAnimationComplete;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        boss.OnSpikeAttackChosen += Boss_OnSpikeAttackChosen;
    }

    private void Boss_OnSpikeAttackChosen()
    {
        animator.SetTrigger("isAttacking");
        StartCoroutine(SpikeAnimationCoroutine());
    }

    private IEnumerator SpikeAnimationCoroutine()
    {
        yield return new WaitForSeconds(0.4f);
        OnBossAnimationComplete?.Invoke();
    }

    private void OnDestroy()
    {
        boss.OnSpikeAttackChosen -= Boss_OnSpikeAttackChosen;
    }
}