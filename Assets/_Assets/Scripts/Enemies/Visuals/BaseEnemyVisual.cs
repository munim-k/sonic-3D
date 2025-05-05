using UnityEngine;
using UnityEngine.UI;

public class BaseEnemyVisual : MonoBehaviour
{

    [SerializeField] private BaseEnemy enemy;
    [SerializeField] private Image healthBar;

    private void Start()
    {
        enemy.onDamage += OnDamage;
    }

    protected virtual void OnDamage()
    {
        healthBar.fillAmount = (float)enemy.GetHealth() / 100f;
    }

    protected virtual void OnDeath()
    {

    }

     private void OnDestroy()
    {
        enemy.onDamage -= OnDamage;
    }


}
