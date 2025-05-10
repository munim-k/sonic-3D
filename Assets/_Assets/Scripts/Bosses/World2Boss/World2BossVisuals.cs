using UnityEngine;
using UnityEngine.UI;

public class World2BossVisuals : MonoBehaviour
{
    [SerializeField] private World2Boss boss;
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private World2Boss.State state;
    void Start()
    {
        boss.OnStateChange += OnStateChange;
        boss.OnDamage += OnDamage;
    }


    private void OnStateChange(World2Boss.State s)
    {
        state = s;
        switch (state)
        {
            case World2Boss.State.Idle:
                shieldVisual.SetActive(true);
                break;
            case World2Boss.State.Attack1:
                shieldVisual.SetActive(true);
                break;
            case World2Boss.State.Attack2:
                shieldVisual.SetActive(true);
                break;
            case World2Boss.State.Stunned:
                shieldVisual.SetActive(false);
                break;
            case World2Boss.State.Dead:
                shieldVisual.SetActive(false);
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
}
