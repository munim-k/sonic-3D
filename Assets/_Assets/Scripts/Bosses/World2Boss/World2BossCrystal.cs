using UnityEngine;

public class World2BossCrystal : MonoBehaviour,BaseEnemy
{
    [SerializeField] private World2Boss boss;
    [SerializeField] private MeshRenderer crystalRenderer;
    [SerializeField] private Material crystalMaterial;
    [SerializeField] private Material crystalMaterialCracked;

    bool isCracked = false;

    private void Awake()
    {
        crystalRenderer.material = crystalMaterialCracked;
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

    public void DoDamageToEnemy(int damage)
    {
        if (!isCracked)
        {
            boss.StunBoss();
        }
    }

    public float GetHealthNormalized()
    {
        return 0f;
    }
}
