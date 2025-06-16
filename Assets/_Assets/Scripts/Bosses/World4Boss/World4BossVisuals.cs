using UnityEngine;
using UnityEngine.UI;

public class World4BossVisuals : MonoBehaviour
{
    [SerializeField] private World4Boss boss;
    [SerializeField] private Image healthBar;
    [SerializeField] private Material spikeActiveMat;
    [SerializeField] private Material spikeInactiveMat;
    [SerializeField] private MeshRenderer[] spikeRenderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private World4Boss.State state;
    void Start()
    {
        boss.OnStateChange += OnStateChange;
        boss.OnDamage += OnDamage;
    }


    private void OnStateChange(World4Boss.State s)
    {
        state = s;
        switch (state)
        {
            case World4Boss.State.Spikes:
                SetSpikeState(true);
                break;
            case World4Boss.State.ProjectileAttacks:
                SetSpikeState(false);
                break;
            case World4Boss.State.Dead:
                SetSpikeState(false);
                break;
            default:
                break;
        }
    }

    private void OnDamage()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = boss.GetHealthNormalized();
        }
    }

    private void SetSpikeState(bool damageable)
    {
        if (damageable)
        {
            foreach (MeshRenderer spike in spikeRenderers)
            {
                spike.material = spikeActiveMat;
            }
        }
        else
        {
            foreach (MeshRenderer spike in spikeRenderers)
            {
                spike.material = spikeInactiveMat;
            }
        }
    }
}
