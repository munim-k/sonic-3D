using System;
using System.Collections;
using UnityEngine;

public class SpikesAnimation : MonoBehaviour
{
    [SerializeField] private BossAnimation bossAnimation;
    public event Action OnSpikeAnimationComplete;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        bossAnimation.OnBossAnimationComplete += BossAnimation_OnBossAnimationComplete;
    }

    private void BossAnimation_OnBossAnimationComplete()
    {
        animator.SetTrigger("isAttacking");
        StartCoroutine(SpikeAnimationComplete());
    }

    private IEnumerator SpikeAnimationComplete()
    {
        yield return new WaitForSeconds(1.2f);
        OnSpikeAnimationComplete?.Invoke();
    }

    private void OnDestroy()
    {
        bossAnimation.OnBossAnimationComplete -= BossAnimation_OnBossAnimationComplete;
    }
}
