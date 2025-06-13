using System;
using UnityEngine;

public class World2BossCrystal : MonoBehaviour,IHittable
{
    [SerializeField] private World2Boss boss;
    [SerializeField] private MeshRenderer crystalRenderer;
    [SerializeField] private Material crystalMaterial;
    [SerializeField] private Material crystalMaterialCracked;
    private Action OnHit;
  
    Action IHittable.OnHit
    {
        get => OnHit;
        set => OnHit = value;
    }
    bool isCracked = false;

    private void Awake()
    {
    }
    public void SetCrystalMaterial(bool cracked)
    {
        isCracked = cracked;
        if (isCracked)
        {
            crystalRenderer.material = crystalMaterialCracked;
        }
        else
        {
            crystalRenderer.material = crystalMaterial;
        }
    }

    public void DoHit(int damage)
    {
        OnHit?.Invoke();
        if (!isCracked)
        {
            boss.StunBoss();
            SetCrystalMaterial(true);
        }
    }
    HittableType IHittable.GetType()
    {
        return HittableType.Obstacle;
    }

    public float GetHealthNormalized()
    {
        return 0f;
    }
}
