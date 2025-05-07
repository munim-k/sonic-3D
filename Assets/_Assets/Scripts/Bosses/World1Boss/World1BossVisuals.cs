using UnityEngine;
using UnityEngine.UI;

public class World1BossVisuals : MonoBehaviour
{

    [SerializeField] private World1Boss boss;
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private World1Boss.State state;
    void Start()
    {
        boss.OnStateChange += OnStateChange;
        boss.OnDamage += OnDamage;
    }


    private void OnStateChange(World1Boss.State s)
    {
        state = s;
        switch (state)
        {
            case World1Boss.State.Spinning:
                shieldVisual.SetActive(true);

                break;
            case World1Boss.State.Stunned:
                shieldVisual.SetActive(false);
                break;
            case World1Boss.State.Dead:
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
            healthBar.fillAmount =boss.GetHealthNormalized();
        }
    }


}
